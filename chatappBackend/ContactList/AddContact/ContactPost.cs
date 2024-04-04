using chatappBackend.Data;
using chatappBackend.User.CreateUser;
using chatappBackend.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace chatappBackend.ContactList.AddContact
{
    [Route("api/User/ContactList")]
    [ApiController]
    public class ContactPost : ControllerBase
    {
        private readonly UserDataContext _context;

        public ContactPost(UserDataContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<ContactsList>> Post(ContactPostRequest request)
        {

            var newContact = new ContactsList
            {
                LoggedUserId = request.LoggedUserId,
                SecondaryUserId = request.SecondaryUserId,
            };
            _context.ContactList.Add(newContact);
            await _context.SaveChangesAsync();
            return Ok(newContact);
        }
    }
}
