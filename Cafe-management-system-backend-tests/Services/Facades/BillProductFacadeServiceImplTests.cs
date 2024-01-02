using AutoFixture;
using Cafe_management_system_backend.MVC.Models;
using Cafe_management_system_backend.MVC.Models.Dtos;
using Cafe_management_system_backend.MVC.Repositories;
using Cafe_management_system_backend.MVC.Security;
using Cafe_management_system_backend.MVC.Services;
using Cafe_management_system_backend.MVC.Services.Facades;
using Moq;

namespace Cafe_management_system_backend_tests.Services.Facades
{
    [TestClass]
    public class BillProductFacadeServiceImplTests
    {
        private Mock<BillProductRepository> mockBillProductRepository;
        private Mock<ProductService> mockProductService;
        private Mock<BillService> mockBillService;
        private Mock<CategoryService> mockCategoryService;
        private BillProductFacadeServiceImpl billProductFacadeService;
        private Fixture fixture;
        private Bill mockBill;
        private List<Product> mockProducts;
        private List<BillProduct> mockBillProducts;
        private GenerateBillReportDto mockGenerateBillReportDto;
        private PrincipalProfile principalProfile;

        [TestInitialize]
        public void Initialize()
        {
            mockBillProductRepository = new Mock<BillProductRepository>();
            mockProductService = new Mock<ProductService>();
            mockBillService = new Mock<BillService>();
            mockCategoryService = new Mock<CategoryService>();

            billProductFacadeService = new BillProductFacadeServiceImpl(
                mockBillProductRepository.Object,
                mockProductService.Object,
                mockBillService.Object,
                mockCategoryService.Object
            );
            fixture = new Fixture();
            // Handle circular refences loop
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            // Random Objects
            mockBillProducts = fixture.CreateMany<BillProduct>(5).ToList();
            mockBill = fixture.Create<Bill>();
            mockProducts = fixture.CreateMany<Product>(5).ToList();
            mockGenerateBillReportDto = new GenerateBillReportDto
            {
                Bill = mockBill,
                Products = mockProducts
            };
            principalProfile = fixture.Create<PrincipalProfile>();

        }

        [TestMethod]
        public void FindBillProductsByBillId_ShouldReturnList_WhenBillProductsExist()
        {
            // Arrange
            int billId = 1;
            mockBillProductRepository.Setup(repo => repo.FindAllByBillId(billId)).Returns(mockBillProducts);

            // Act
            var result = billProductFacadeService.FindBillProductsByBillId(billId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(mockBillProducts, result);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void FindBillProductsByBillId_ShouldThrowKeyNotFoundException_WhenBillProductsDoNotExist()
        {
            // Arrange
            int billId = 1;
            mockBillProductRepository.Setup(repo => repo.FindAllByBillId(billId)).Returns(new List<BillProduct>());

            // Act & Assert
            billProductFacadeService.FindBillProductsByBillId(billId);
        }

        [TestMethod]
        public void DeleteBillProductAndBill_ShouldDeleteBillProductAndBill_WhenBillExists()
        {
            // Arrange
            string billUuid = "some-uuid";
            mockBill.uuid = billUuid;
            mockBillService.Setup(service => service.FindBillByUUID(billUuid)).Returns(mockBill);
            mockBillProductRepository.Setup(repo => repo.FindAllByBillId(mockBill.id)).Returns(mockBillProducts);

            // Act
            billProductFacadeService.DeleteBillProductAndBill(billUuid);

            // Assert
            mockBillProductRepository.Verify(repo => repo.Delete(mockBillProducts), Times.Once);
            mockBillService.Verify(service => service.DeleteBill(billUuid), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void DeleteBillProductAndBill_ShouldThrowKeyNotFoundException_WhenBillDoesNotExist()
        {
            // Arrange
            string nonExistingBillUuid = "non_existing_uuid";
            mockBillService.Setup(service => service.FindBillByUUID(nonExistingBillUuid))
                .Throws(new KeyNotFoundException());

            // Act
            billProductFacadeService.DeleteBillProductAndBill(nonExistingBillUuid);

            // Assert
            // Expects KeyNotFoundException to be thrown
        }

        [TestMethod]
        public void GenerateBillReport_ShouldGenerateBillAndPdf_WhenProductsExist()
        {
            // Arrange
            mockProductService.Setup(service => service.DoesProductExistById(It.IsAny<int>())).Returns(true);

            // Act
            var result = billProductFacadeService.GenerateBillReport(principalProfile, mockGenerateBillReportDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof((Bill, byte[])));
            Assert.IsNotNull(result.Item1);
            Assert.IsNotNull(result.Item2);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void GenerateBillReport_ShouldThrowException_WhenProductsDoNotExist()
        {
            // Arrange
            mockProductService.Setup(service => service.DoesProductExistById(It.IsAny<int>())).Throws(new KeyNotFoundException());

            // Act & Assert
            billProductFacadeService.GenerateBillReport(principalProfile, mockGenerateBillReportDto);
        }
    }
}
