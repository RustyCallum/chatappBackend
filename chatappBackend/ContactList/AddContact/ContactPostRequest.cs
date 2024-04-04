namespace chatappBackend.ContactList.AddContact
{
    public class ContactPostRequest
    {
        public int LoggedUserId { get; set; }
        public int SecondaryUserId { get; set; }
    }
}
