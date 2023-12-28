namespace Cafe_management_system_backend.MVC.Security
{
    // All the info we want to for the 'Change Password' process,
    // represented in an object form
    public class ChangePassword
    {
        public string oldPassword { get; set; }
        public string newPassword { get; set; }
    }
}