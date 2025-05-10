using Todoapp.Context;
using Todoapp.Entities;

namespace Todoapp.Services
{
    public interface IAuthServices
    {

        Task<User?> RegisterAsync(UserDTO request);
        Task<string?> LoginAsync(UserDTO request); 
    }
}