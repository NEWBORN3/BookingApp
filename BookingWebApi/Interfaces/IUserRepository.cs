using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Interfaces
{
    public interface IUserRepository
    {
         Task<User> Authenticate(string username, string password);
         void Register(string username, string password);
         Task<bool> UserAlreadyExists(string username);
    }
}