using Cafe_management_system_backend.Models;

namespace Cafe_management_system_backend.Services
{
    public interface UserService
    {
        void SignUp(User user);
        object Login(User user);
    }
}
