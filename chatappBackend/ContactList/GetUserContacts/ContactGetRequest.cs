namespace chatappBackend.ContactList.GetUserContacts
{
    public class ContactGetRequest
    {
        public Guid LoggedUserId { get; set; }
        public string Guid { get; set; }
    }
}
