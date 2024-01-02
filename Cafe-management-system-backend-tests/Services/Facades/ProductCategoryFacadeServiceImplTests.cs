using Moq;
using AutoFixture;
using Cafe_management_system_backend.MVC.Models;
using Cafe_management_system_backend.MVC.Services.Facades;
using Cafe_management_system_backend.MVC.Services;

#pragma warning disable
namespace Cafe_management_system_backend_tests.Services.Facades
{
    [TestClass]
    public class ProductCategoryFacadeServiceImplTests
    {
        private Mock<CategoryService> mockCategoryService;
        private Mock<ProductService> mockProductService;
        private ProductCategoryFacadeServiceImpl productCategoryFacadeService;
        private Fixture fixture;

        [TestInitialize]
        public void Initialize()
        {
            mockCategoryService = new Mock<CategoryService>();
            mockProductService = new Mock<ProductService>();
            productCategoryFacadeService = new ProductCategoryFacadeServiceImpl(mockCategoryService.Object, mockProductService.Object);
            fixture = new Fixture();
        }

        [TestMethod]
        public void AddProductWithCategory_ShouldAddProduct_WhenCategoryExists()
        {
            // Arrange
            var product = fixture.Create<Product>();
            var categoryDB = fixture.Create<Category>();
            mockCategoryService.Setup(service => service.FindCategoryById(product.categoryId)).Returns(categoryDB);

            // Act
            productCategoryFacadeService.AddProductWithCategory(product);

            // Assert
            mockProductService.Verify(service => service.AddProduct(product), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void AddProductWithCategory_ShouldThrowException_WhenCategoryDoesNotExist()
        {
            // Arrange
            var product = fixture.Create<Product>();
            mockCategoryService.Setup(service => service.FindCategoryById(product.categoryId)).Returns((Category)null);

            // Act
            productCategoryFacadeService.AddProductWithCategory(product);
        }

        [TestMethod]
        public void GetProductsByCategoryIdAndStatus_ShouldReturnProducts_WhenCategoryExists()
        {
            // Arrange
            var categoryId = 123;
            var productStatus = "true";

            // Assuming fixture is correctly initialized
            var categoryDB = fixture.Build<Category>().Create();
            var expectedProducts = fixture.CreateMany<Product>(3).ToList();

            mockCategoryService.Setup(service => service.FindCategoryById(categoryId)).Returns(categoryDB);
            mockProductService.Setup(service => service.FindProductsByCategoryIdAndStatus(categoryId, productStatus)).Returns(expectedProducts);

            // Act
            var result = productCategoryFacadeService.GetProductsByCategoryIdAndStatus(categoryId, productStatus);

            // Assert
            Assert.IsNotNull(categoryDB, "Category should exist.");
            CollectionAssert.AreEqual(expectedProducts, result.ToList());
        }



        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void GetProductsByCategoryIdAndStatus_ShouldThrowException_WhenCategoryDoesNotExist()
        {
            // Arrange
            var categoryId = 123;
            var productStatus = "true";
            mockCategoryService.Setup(service => service.FindCategoryById(categoryId)).Returns((Category)null);

            // Act
            productCategoryFacadeService.GetProductsByCategoryIdAndStatus(categoryId, productStatus);
        }
    }
}
