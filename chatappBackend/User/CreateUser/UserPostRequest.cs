namespace chatappBackend.User.CreateUser
{
    public class UserPostRequest
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
