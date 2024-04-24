using chatappBackend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace chatappBackend.User.CreateUser
{
    [AllowAnonymous]
    [Route("api/User/Register")]
    [ApiController]
    public class UserPost : ControllerBase
    {
        private readonly ILogger<UserPost> _logger;

        private readonly UserDataContext _context;

        public UserPost(UserDataContext context, ILogger<UserPost> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<Users>> Post(UserPostRequest request)
        {
            _logger.LogInformation($"User with username {request.Username} is trying to register");

            CreatePassHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            _logger.LogInformation($"Salt and hash succesfully created for user {request.Username}");

            var newUser = new Users
            {
                Id = Guid.NewGuid(),
                UserName = request.Username,
                Hash = passwordHash,
                Salt = passwordSalt,
                Role = "User"
            };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"User with username {newUser.UserName} sucessfully registered");

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
