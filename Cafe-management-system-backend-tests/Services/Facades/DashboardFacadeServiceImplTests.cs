using Cafe_management_system_backend.MVC.Models.Dtos.Dashboard;
using Cafe_management_system_backend.MVC.Services;
using Cafe_management_system_backend.MVC.Services.Facades;
using Cafe_management_system_backend.MVC.Services.UserServices;
using Moq;

namespace Cafe_management_system_backend_tests.Services.Facades
{
    [TestClass]
    public class DashboardFacadeServiceImplTests
    {
        private Mock<CategoryService> mockCategoryService;
        private Mock<ProductService> mockProductService;
        private Mock<BillService> mockBillService;
        private Mock<UserService> mockUserService;
        private Mock<DashboardServiceCustom> mockDashboardService;
        private DashboardFacadeServiceImpl dashboardFacadeService;

        [TestInitialize]
        public void Initialize()
        {
            mockCategoryService = new Mock<CategoryService>();
            mockProductService = new Mock<ProductService>();
            mockBillService = new Mock<BillService>();
            mockUserService = new Mock<UserService>();
            mockDashboardService = new Mock<DashboardServiceCustom>();

            dashboardFacadeService = new DashboardFacadeServiceImpl(
                mockCategoryService.Object,
                mockProductService.Object,
                mockBillService.Object,
                mockUserService.Object,
                mockDashboardService.Object
            );
        }

        [TestMethod]
        public void GetDashboardCounts_ShouldReturnDashboardCountsDto()
        {
            // Arrange
            int categoriesCount = 5;
            int productsCount = 10;
            int billsCount = 15;
            int usersCount = 20;

            DashboardCountsDto expectedDashboardCountsDto = new DashboardCountsDto(categoriesCount, productsCount, billsCount, usersCount);
            mockCategoryService.Setup(service => service.CountAllCategories()).Returns(categoriesCount);
            mockProductService.Setup(service => service.CountAllProducts()).Returns(productsCount);
            mockBillService.Setup(service => service.CountAllBills()).Returns(billsCount);
            mockUserService.Setup(service => service.CountAllUsers()).Returns(usersCount);
            mockDashboardService.Setup(service =>
                    service.GetDashboardCounts(categoriesCount, productsCount, billsCount, usersCount))
                .Returns(expectedDashboardCountsDto);

            // Act
            var result = dashboardFacadeService.getDashboardCounts();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedDashboardCountsDto, result);
            mockDashboardService.Verify(service =>
                service.GetDashboardCounts(categoriesCount, productsCount, billsCount, usersCount), Times.Once);
        }

    }
}
