using chatappBackend.Data;
using chatappBackend.User.CreateUser;
using chatappBackend.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace chatappBackend.ContactList.AddContact
{
    [Route("api/User/ContactList")]
    [ApiController]
    public class ContactPost : ControllerBase
    {
        private readonly ILogger<ContactPost> _logger;

        private readonly UserDataContext _context;

        public ContactPost(UserDataContext context, ILogger<ContactPost> logger)
        {
            _context = context;
            _logger = logger;
        } 

        [HttpPost]
        public async Task<ActionResult<ContactsList>> Post(ContactPostRequest request)
        {
            var JWTHandler = new JwtSecurityTokenHandler();
            var token = Request.Cookies["token"];
            var jsonToken = JWTHandler.ReadToken(token);
            var tokenDecrypted = jsonToken as JwtSecurityToken;

            var UserName = tokenDecrypted.Claims.First(claim => claim.Type == "UserName").Value;
            var UserId = tokenDecrypted.Claims.First(claim => claim.Type == "ClientId").Value;
            var Guid = tokenDecrypted.Claims.First(claim => claim.Type == "X-CSRF-TOKEN").Value;

            _logger.LogInformation($"User {request.LoggedUserId} is trying to add contact.");

            if(Guid == request.Guid)
            {
                var newContact = new ContactsList
                {
                    LoggedUserId = request.LoggedUserId,
                    SecondaryUserId = request.SecondaryUserId,
                };

                if (_context.ContactList.Where(x => x.LoggedUserId == request.LoggedUserId && x.SecondaryUserId == request.SecondaryUserId).Any())
                {
                    return BadRequest("This contact already exists");
                }
                else
                {
                    _context.ContactList.Add(newContact);
                    _logger.LogInformation($"User {request.LoggedUserId} is added user {request.SecondaryUserId} as a contact.");
                    await _context.SaveChangesAsync();
                    return Ok(newContact);
                }
            }
            else
            {
                return BadRequest("Wrong GUID");
            }
        }
    }
}
