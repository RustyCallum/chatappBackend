namespace chatappBackend.Message.Create
{
    public class CreateMessageRequest
    {
        public string Guid { get; set; }
        public Guid MessageId { get; set; }
        public Guid LoggedUserId {  get; set; }
        public string MessageBody { get; set; }
        public Guid SecondaryUserId { get; set; }
    }
}
