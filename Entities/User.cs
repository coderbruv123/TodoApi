using System.Text.Json.Serialization;

namespace Todoapp.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public  string Username { get; set; }= string.Empty;
        public string Password { get; set; } = string.Empty;
    
        [JsonIgnore]
        public  ICollection<Todo> Todos { get; set; } = new List<Todo>();
    }
}