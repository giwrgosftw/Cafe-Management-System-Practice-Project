using Cafe_management_system_backend.MVC.Security;
using NLog;
using System;

namespace Cafe_management_system_backend.MVC.Services.UserServices
{
    public class UserAuthorityServiceImpl : UserAuthorityService
    {
        private static Logger logger = LogManager.GetLogger("NLogger");

        /* 
         * This method is intended to verify if the user who makes the API call possesses the "Admin" role rights.
         * TODO: Instead of implementing a dedicated method for this purpose, a more robust approach
         * would involve setting up three database tables: Role -> PermissionRole <- Permission. 
         * Then, leverage authorization attributes like [Authorize(Roles = "ADMIN_VIEW")] in our controllers.
         */
        public bool HasAuthorityAdmin(string token)
        {
            try
            {
                PrincipalProfile principalProfile = TokenManager.GetPrincipalProfileInfo(token);
                if (principalProfile.Role != UserRoleEnum.Admin.ToString())
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.Error($"[Method:HasAuthorityAdmin()] Exception: {ex.Message}");
                throw;
            }
        }
    }
}