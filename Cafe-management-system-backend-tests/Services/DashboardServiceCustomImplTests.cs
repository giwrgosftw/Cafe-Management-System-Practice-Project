using Cafe_management_system_backend.MVC.Models.Dtos.Dashboard;
using Cafe_management_system_backend.MVC.Repositories;
using Cafe_management_system_backend.MVC.Services;
using Moq;

namespace Cafe_management_system_backend_tests.Services
{
    [TestClass]
    public class DashboardServiceCustomImplTests
    {
        private Mock<DashboardRepositoryCustom> mockDashboardRepository;
        private DashboardServiceCustomImpl dashboardServiceCustom;

        [TestInitialize]
        public void Initialize()
        {
            mockDashboardRepository = new Mock<DashboardRepositoryCustom>();
            dashboardServiceCustom = new DashboardServiceCustomImpl(mockDashboardRepository.Object);
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
            mockDashboardRepository.Setup(repo =>
                    repo.GetDashboardCountsDto(categoriesCount, productsCount, billsCount, usersCount))
                .Returns(expectedDashboardCountsDto);

            // Act
            var result = dashboardServiceCustom.GetDashboardCounts(categoriesCount, productsCount, billsCount, usersCount);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedDashboardCountsDto, result);
            mockDashboardRepository.Verify(repo =>
                repo.GetDashboardCountsDto(categoriesCount, productsCount, billsCount, usersCount), Times.Once);
        }
    }
}
