using Microsoft.Identity.Client;

namespace chatappBackend.Message
{
    public class Messages
    {
        public Guid Id { get; set; }
        public string MessageBody { get; set; }
        public Guid LoggedUserId { get; set; }
        public Guid SecondaryUserId { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
