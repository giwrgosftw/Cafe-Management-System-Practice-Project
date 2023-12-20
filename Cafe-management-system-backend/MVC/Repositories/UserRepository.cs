using Cafe_management_system_backend.MVC.Models;
using System;

namespace Cafe_management_system_backend.MVC.Repositories
{
    public interface UserRepository
    {
        void AddUser(User user);
        User FindByEmail(String userEmail);
        User FindByEmailAndPassword(String userEmail, String userPassword);
        
    }
}
