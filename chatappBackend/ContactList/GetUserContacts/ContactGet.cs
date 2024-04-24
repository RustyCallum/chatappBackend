using chatappBackend.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace chatappBackend.ContactList.GetUserContacts
{
    [Route("api/User/ContactList")]
    [ApiController]
    public class ContactGet : ControllerBase
    {
        private readonly ILogger<ContactGet> _logger;

        private readonly UserDataContext _context;
        private List<ContactsList> _contactList;
        private List<Contact> _contacts;

        public ContactGet(UserDataContext context, ILogger<ContactGet> logger)
        {
            _context = context;
            _contacts = new List<Contact>();
            _contactList = new List<ContactsList>();
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContactsList>> GetContacts(Guid id)
        {
            _logger.LogInformation($"User with id {id} is trying to access his contacts");

            var JWTHandler = new JwtSecurityTokenHandler();
            var token = Request.Cookies["token"];
            var jsonToken = JWTHandler.ReadToken(token);
            var tokenDecrypted = jsonToken as JwtSecurityToken;

            var Guid = tokenDecrypted.Claims.First(claim => claim.Type == "X-CSRF-TOKEN").Value;



            _contactList = _context.ContactList
                .Where(x => x.LoggedUserId == id)
                .ToList();

            for (int i = 0; i < _contactList.Count; i++)
            {
                var tmp = new Contact
                {
                    Id = _contactList[i].Id,
                    Name = _context.Users
                        .Where(x => x.Id == _contactList[i].SecondaryUserId)
                        .Select(y => y.UserName)
                        .FirstOrDefault()
                };

                _contacts.Add(tmp);
            }

            return Ok(_contacts);
        }
    }
}
