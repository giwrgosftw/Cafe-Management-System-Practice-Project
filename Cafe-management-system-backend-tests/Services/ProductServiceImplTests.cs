using Moq;
using AutoFixture;
using Cafe_management_system_backend.MVC.Models;
using Cafe_management_system_backend.MVC.Repositories;
using Cafe_management_system_backend.MVC.Services;
using System.Data;

namespace Cafe_management_system_backend_tests.Services
{
    [TestClass]
    public class ProductServiceImplTests
    {
        private Mock<ProductRepository> mockProductRepository;
        private ProductServiceImpl productService;
        private Fixture fixture;

        [TestInitialize]
        public void Initialize()
        {
            mockProductRepository = new Mock<ProductRepository>();
            productService = new ProductServiceImpl(mockProductRepository.Object);
            fixture = new Fixture();
        }

        [TestMethod]
        public void AddProduct_ShouldAddNewProduct_WhenProductDoesNotExist()
        {
            // Arrange
            var product = fixture.Create<Product>();
            mockProductRepository.Setup(repo => repo.FindByName(product.name)).Returns((Product)null);

            // Act
            productService.AddProduct(product);

            // Assert
            mockProductRepository.Verify(repo => repo.Add(product), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(DuplicateNameException))]
        public void AddProduct_ShouldThrowException_WhenProductAlreadyExists()
        {
            // Arrange
            var product = fixture.Create<Product>();
            mockProductRepository.Setup(repo => repo.FindByName(product.name)).Returns(product);

            // Act
            productService.AddProduct(product);
        }

        [TestMethod]
        public void UpdateProduct_ShouldUpdateProduct_WhenProductExists()
        {
            // Arrange
            var product = fixture.Create<Product>();
            mockProductRepository.Setup(repo => repo.FindById(product.id)).Returns(product);

            // Act
            productService.UpdateProduct(product);

            // Assert
            mockProductRepository.Verify(repo => repo.Update(product), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void UpdateProduct_ShouldThrowException_WhenProductDoesNotExist()
        {
            // Arrange
            var product = fixture.Create<Product>();
            mockProductRepository.Setup(repo => repo.FindById(product.id)).Returns((Product)null);

            // Act
            productService.UpdateProduct(product);
        }

        [TestMethod]
        public void FindProductById_ShouldReturnProduct_WhenProductExists()
        {
            // Arrange
            var product = fixture.Create<Product>();
            mockProductRepository.Setup(repo => repo.FindById(product.id)).Returns(product);

            // Act
            var result = productService.FindProductById(product.id);

            // Assert
            Assert.AreEqual(product, result);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void FindProductById_ShouldThrowException_WhenProductDoesNotExist()
        {
            // Arrange
            int productId = 1;
            mockProductRepository.Setup(repo => repo.FindById(productId)).Returns((Product)null);

            // Act
            productService.FindProductById(productId);
        }

        [TestMethod]
        public void FindAllProducts_ShouldReturnListOfProducts()
        {
            // Arrange
            var expectedProducts = fixture.CreateMany<Product>(5).ToList();
            mockProductRepository.Setup(repo => repo.FindAll()).Returns(expectedProducts);

            // Act
            var result = productService.FindAllProducts();

            // Assert
            CollectionAssert.AreEqual(expectedProducts, result);
        }

        [TestMethod]
        public void FindProductsByCategoryIdAndStatus_ShouldReturnListOfProducts()
        {
            // Arrange
            int categoryId = 1;
            string productStatus = "true";
            var expectedProducts = fixture.CreateMany<Product>(3).ToList();
            mockProductRepository.Setup(repo => repo.FindAllByCategoryIdAndStatus(categoryId, productStatus)).Returns(expectedProducts);

            // Act
            var result = productService.FindProductsByCategoryIdAndStatus(categoryId, productStatus);

            // Assert
            CollectionAssert.AreEqual(expectedProducts, result);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void DeleteProduct_ShouldThrowException_WhenProductDoesNotExist()
        {
            // Arrange
            int productId = 1;
            mockProductRepository.Setup(repo => repo.FindById(productId)).Returns((Product)null);

            // Act
            productService.DeleteProduct(productId);
        }

        [TestMethod]
        public void DeleteProduct_ShouldDeleteProduct_WhenProductExists()
        {
            // Arrange
            var product = fixture.Create<Product>();
            mockProductRepository.Setup(repo => repo.FindById(product.id)).Returns(product);

            // Act
            productService.DeleteProduct(product.id);

            // Assert
            mockProductRepository.Verify(repo => repo.Delete(product), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void AddProduct_ShouldThrowException_WhenProductNameIsNull()
        {
            // Arrange
            var product = fixture.Create<Product>();
            product.name = null;

            // Act
            productService.AddProduct(product);
        }

        [TestMethod]
        public void DoesProductExistById_ShouldReturnTrue_WhenProductExists()
        {
            // Arrange
            int productId = 1;
            mockProductRepository.Setup(repo => repo.DoesExistById(productId)).Returns(true);

            // Act
            var result = productService.DoesProductExistById(productId);

            // Assert
            Assert.IsTrue(result);
        }


        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void DoesProductExistById_ShouldThrowException_WhenProductDoesNotExist()
        {
            // Arrange
            int productId = 1;
            mockProductRepository.Setup(repo => repo.DoesExistById(productId)).Returns(false);

            // Act
            productService.DoesProductExistById(productId);
        }

    }
}
