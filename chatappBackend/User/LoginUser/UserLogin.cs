using chatappBackend.Data;
using chatappBackend.User.CreateUser;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace chatappBackend.User.LoginUser
{
    [Route("api/User/Login")]
    [ApiController]
    public class UserLogin : ControllerBase
    {
        private const string TokenSecretKey = "Aleksander51HaHaXDbekazwasxDxDnienawidzewas";

        private readonly UserDataContext _context;

        public UserLogin(UserDataContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<ActionResult<string>> Login(UserPostRequest user)
        {
            Guid g = Guid.NewGuid();

            if (user == null)
            {
                return BadRequest("User doesn't exist");
            }
            else
            {
                var userToCheck = _context.Users.Where(x => x.UserName == user.Username).FirstOrDefault();
                if (userToCheck == null)
                {
                    return BadRequest("User doesn't exist");
                }
                if (!VerifyPassword(user.Password, userToCheck.Hash, userToCheck.Salt))
                {
                    return BadRequest("Wrong password");
                }
                string token = GenerateToken(userToCheck, g);
                return Ok(userToCheck);
            }
        }
        private bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private string GenerateToken(Users request, Guid g)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(TokenSecretKey);

            var claims = new List<Claim>
            {
                new("UserName", request.UserName),
                new("ClientId", request.Id.ToString()),
                new("X-CSRF-TOKEN", g.ToString())
            };

            var TokenSettings = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(20),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(TokenSettings);
            var jwt = tokenHandler.WriteToken(token);

            Response.Headers.Append("X-CSRF-TOKEN", g.ToString());

            Response.Cookies.Append("token", jwt,
                new CookieOptions
                {
                    Expires = DateTime.Now.AddHours(1),
                    HttpOnly = true,
                    Secure = true,
                    IsEssential = true,
                    SameSite= Microsoft.AspNetCore.Http.SameSiteMode.None
                });

            return jwt;
        }
    }
}
