using Cafe_management_system_backend.MVC.Models;
using Cafe_management_system_backend.MVC.Security;

namespace Cafe_management_system_backend.MVC.Services.UserServices
{
    public interface UserService
    {
        void SignUp(User user);
        object Login(User user);
        User UpdateUser(User user);
        User ChangeUserPassword(PrincipalProfile principal, ChangePassword changePassword);
        void DeleteUser(int userId);
        void DeleteMyAccount(string principalEmail);
    }
}
