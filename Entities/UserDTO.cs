namespace Todoapp.Entities
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        public  string Username { get; set; }= string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}