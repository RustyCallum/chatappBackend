using chatappBackend.ContactList.GetUserContacts;
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
        private readonly UserDataContext _context;
        private List<Contact> _contacts;

        public ContactGetByName (UserDataContext context)
        {
            _context = context;
            _contacts = new List<Contact>();
        }

        [HttpPost]
        public async Task<IActionResult> Post(ContactGetByNameRequest request)
        {
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
                return Ok(_contacts);
            }

            else
            {
                return BadRequest("Not authenticated action");
            }
        }
    }
}
