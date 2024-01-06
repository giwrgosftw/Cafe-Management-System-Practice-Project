using Cafe_management_system_backend.MVC.Models.Dtos.Dashboard;

namespace Cafe_management_system_backend.MVC.Services
{
    public interface DashboardServiceCustom
    {
        DashboardCountsDto GetDashboardCounts(int categoriesCount, int productsCount, int billsCount, int usersCount);
    }
}
