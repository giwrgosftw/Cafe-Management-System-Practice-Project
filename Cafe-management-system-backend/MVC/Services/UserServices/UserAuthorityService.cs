namespace Cafe_management_system_backend.MVC.Services.UserServices
{
    public interface UserAuthorityService
    {
        bool HasAuthorityAdmin(string token);
    }
}