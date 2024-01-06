using Cafe_management_system_backend.MVC.Models.Dtos.Dashboard;

namespace Cafe_management_system_backend.MVC.Repositories
{
    public class DashboardRepositoryCustomImpl : DashboardRepositoryCustom
    {
        private DashboardDto dashboardDto;

        public DashboardRepositoryCustomImpl(DashboardDto dashboardDto)
        {
            this.dashboardDto = dashboardDto;
        }

        /// <summary>Generates a DashboardCountsDto object (part of Dashboard Object data) 
        /// with the specified counts for categories, products, bills, and users.
        /// </summary>
        /// <param name="categoriesCount">Count of categories.</param>
        /// <param name="productsCount">Count of products.</param>
        /// <param name="billsCount">Count of bills.</param>
        /// <param name="usersCount">Count of users.</param>
        /// <returns>DashboardCountsDto object with the provided counts.</returns>
        public DashboardCountsDto GetDashboardCountsDto(int categoriesCount, int productsCount, int billsCount, int usersCount)
        {
            dashboardDto.SetDashboardCountsDto(new DashboardCountsDto(categoriesCount, productsCount, billsCount, usersCount));
            return dashboardDto.GetDashboardCountsDto();
        }

    }
}