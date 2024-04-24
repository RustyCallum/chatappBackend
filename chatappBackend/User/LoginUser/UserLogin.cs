using chatappBackend.Data;
using chatappBackend.User.CreateUser;
using Microsoft.AspNetCore.Authorization;
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
    [AllowAnonymous]
    [Route("api/User/Login")]
    [ApiController]
    public class UserLogin : ControllerBase
    {
        private const string TokenSecretKey = "Aleksander51HaHaXDbekazwasxDxDnienawidzewas";

        private readonly ILogger<UserLogin> _logger;

        private readonly UserDataContext _context;

        public UserLogin(UserDataContext context, ILogger<UserLogin> logger)
        {
            _context = context;
            _logger = logger;
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
                    _logger.LogInformation($"Someone tried to login with username {userToCheck.UserName}, but this user doesn't exist.");
                    return BadRequest("User doesn't exist");
                }
                if (!VerifyPassword(user.Password, userToCheck.Hash, userToCheck.Salt))
                {
                    _logger.LogInformation($"User with username {userToCheck.UserName} tried to login, but his password was wrong");
                    return BadRequest("Wrong password");
                }
                string token = GenerateToken(userToCheck, g);
                _logger.LogInformation($"User {userToCheck.UserName} successfully logged in");
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
                new("X-CSRF-TOKEN", g.ToString()),
                new(ClaimTypes.Role, request.Role),
            };

            var TokenSettings = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Audience = "http://localhost:7094/",
                Issuer = "http://localhost:7094/",
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
