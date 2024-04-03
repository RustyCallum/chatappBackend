namespace chatappBackend.Message.Create
{
    public class CreateMessageRequest
    {
        public string Guid { get; set; }
        public string MessageBody { get; set; }
        public int RecieverId { get; set; }
    }
}
