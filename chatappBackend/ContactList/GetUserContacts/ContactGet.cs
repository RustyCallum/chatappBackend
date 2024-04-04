using chatappBackend.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace chatappBackend.ContactList.GetUserContacts
{
    [Route("api/User/ContactList")]
    [ApiController]
    public class ContactGet : ControllerBase
    {
        private readonly UserDataContext _context;
        private List<ContactsList> _contactList;
        private List<Contact> _contacts;

        public ContactGet(UserDataContext context)
        {
            _context = context;
            _contacts = new List<Contact>();
            _contactList = new List<ContactsList>();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContactsList>> GetContacts(int id)
        {
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
