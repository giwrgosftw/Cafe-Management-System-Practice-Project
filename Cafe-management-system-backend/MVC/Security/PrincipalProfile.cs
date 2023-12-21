namespace Cafe_management_system_backend.MVC.Security
{
    // All the info we want to get from the Principal,
    // represented in an object form
    public class PrincipalProfile
    {
        public string Email { get; set; }
        public string Role { get; set; }
    }
}