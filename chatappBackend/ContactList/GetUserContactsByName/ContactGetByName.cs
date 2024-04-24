using chatappBackend.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace chatappBackend.ContactList.GetUserContactsByName
{
    [Route("api/User/ContactList/Name")]
    [ApiController]
    public class ContactGetByName : ControllerBase
    {
        private readonly ILogger<ContactGetByName> _logger;

        private readonly UserDataContext _context;
        private List<Contact> _contacts;

        public ContactGetByName (UserDataContext context, ILogger<ContactGetByName> logger)
        {
            _context = context;
            _contacts = new List<Contact>();
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post(ContactGetByNameRequest request)
        {
            _logger.LogInformation($"User is trying to access his contacts by name");

            var JWTHandler = new JwtSecurityTokenHandler();
            var token = Request.Cookies["token"];
            var jsonToken = JWTHandler.ReadToken(token);
            var tokenDecrypted = jsonToken as JwtSecurityToken;

            var Guid = tokenDecrypted.Claims.First(claim => claim.Type == "X-CSRF-TOKEN").Value;

            if(Guid == request.Guid)
            {
                _contacts = _context.Users.Where(x => x.UserName
                .Contains(request.Username))
                .Select(y => new Contact
                {
                    Id = y.Id,
                    Name = y.UserName,
                    Photo = null
                })
                .ToList();
                _logger.LogInformation($"User {request.Username} successfully got his contacts by name");
                return Ok(_contacts);
            }

            else
            {
                _logger.LogInformation($"User {request.Username} unauthenticated tried to access his contacts");
                return BadRequest("Not authenticated action");
            }
        }
    }
}
