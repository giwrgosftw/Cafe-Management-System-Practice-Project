using Cafe_management_system_backend.MVC.Models;
using System.Collections.Generic;

namespace Cafe_management_system_backend.MVC.Services.UserServices
{
    public interface CommonUserService
    {
        List<User> FindAllUsers();
        User FindUserById(int userId);
        User FindUserByEmail(string userEmail);
        User FindUserByEmailAndPassword(string email, string password);
    }
}
