namespace chatappBackend.ContactList.AddContact
{
    public class ContactPostRequest
    {
        public Guid LoggedUserId { get; set; }
        public Guid SecondaryUserId { get; set; }
        public string Guid { get; set; }
    }
}
