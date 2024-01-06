using Cafe_management_system_backend.MVC.Models.Dtos.Dashboard;
using Cafe_management_system_backend.MVC.Repositories;

namespace Cafe_management_system_backend.MVC.Services
{
    public class DashboardServiceCustomImpl : DashboardServiceCustom
    {
        private readonly DashboardRepositoryCustom dashboardRepository;

        public DashboardServiceCustomImpl(DashboardRepositoryCustom dashboardRepository)
        {
            this.dashboardRepository = dashboardRepository;
        }

        /// <summary>Gets a DashboardCountsDto object (part of Dashboard Object data) 
        /// with the specified counts for categories, products, bills, and users.
        /// </summary>
        /// <param name="categoriesCount">Count of categories.</param>
        /// <param name="productsCount">Count of products.</param>
        /// <param name="billsCount">Count of bills.</param>
        /// <param name="usersCount">Count of users.</param>
        /// <returns>DashboardCountsDto object with the provided counts.</returns>
        public DashboardCountsDto GetDashboardCounts(int categoriesCount, int productsCount, int billsCount, int usersCount)
        {
            return dashboardRepository.GetDashboardCountsDto(categoriesCount, productsCount, billsCount, usersCount);
        }
    }
}