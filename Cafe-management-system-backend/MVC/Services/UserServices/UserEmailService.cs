using Cafe_management_system_backend.MVC.Models;
using System.Threading.Tasks;

namespace Cafe_management_system_backend.MVC.Services.UserServices
{
    public interface UserEmailService
    {
        Task SendForgotPasswordEmail(User user);
    }
}
