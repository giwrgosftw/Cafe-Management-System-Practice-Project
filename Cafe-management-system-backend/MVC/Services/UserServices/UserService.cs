using Cafe_management_system_backend.MVC.Models;
using System.Collections.Generic;

namespace Cafe_management_system_backend.MVC.Services.UserServices
{
    public interface UserService
    {
        void SignUp(User user);
        object Login(User user);
        List<User> FindAllUsers();
    }
}
