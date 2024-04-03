using chatappBackend.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace chatappBackend.User.CreateUser
{
    [Route("api/User/Register")]
    [ApiController]
    public class UserPost : ControllerBase
    {
        private readonly UserDataContext _context;

        public UserPost(UserDataContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Users>> Post(UserPostRequest request)
        {
            CreatePassHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var newUser = new Users
            {
                UserName = request.Username,
                Hash = passwordHash,
                Salt = passwordSalt
            };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return Ok(newUser);
        }

        private void CreatePassHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
