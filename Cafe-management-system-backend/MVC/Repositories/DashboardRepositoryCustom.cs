using Cafe_management_system_backend.MVC.Models.Dtos.Dashboard;

namespace Cafe_management_system_backend.MVC.Repositories
{
    public interface DashboardRepositoryCustom
    {
        DashboardCountsDto GetDashboardCountsDto(int categoriesCount, int productsCount, int billsCount, int usersCount);
    }
}
