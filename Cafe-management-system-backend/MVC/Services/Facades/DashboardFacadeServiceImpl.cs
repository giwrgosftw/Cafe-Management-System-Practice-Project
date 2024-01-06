using Cafe_management_system_backend.MVC.Models.Dtos.Dashboard;
using Cafe_management_system_backend.MVC.Services.UserServices;

namespace Cafe_management_system_backend.MVC.Services.Facades
{
    public class DashboardFacadeServiceImpl : DashboardFacadeService
    {
        private readonly CategoryService categoryService;
        private readonly ProductService productService;
        private readonly BillService billService;
        private readonly UserService userService;
        private readonly DashboardServiceCustom dashboardService;

        public DashboardFacadeServiceImpl(CategoryService categoryService, ProductService productService, 
            BillService billService, UserService userService, DashboardServiceCustom dashboardService)
        {
            this.categoryService = categoryService;
            this.productService = productService;
            this.billService = billService;
            this.userService = userService;
            this.dashboardService = dashboardService;
        }

        /// <summary>Retrieves dashboard counts for categories, products, bills, and users.</summary>
        /// <returns>List of integers representing the dashboard counts.</returns>
        public DashboardCountsDto getDashboardCounts()
        {
            return dashboardService.GetDashboardCounts(categoryService.CountAllCategories(), productService.CountAllProducts(), 
                billService.CountAllBills(), userService.CountAllUsers());
        }
    }
}