using Cafe_management_system_backend.MVC.Models;
using Cafe_management_system_backend.MVC.Repositories;
using Cafe_management_system_backend.MVC.Services;
using Moq;
using AutoFixture;
using System.Data;

namespace Cafe_management_system_backend_tests.Services
{
    [TestClass]
    public class CategoryServiceImplTests
    {
        private Mock<CategoryRepository> mockCategoryRepository;
        private CategoryServiceImpl categoryService;
        private Fixture fixture;
        private Category categoryDB;

        [TestInitialize]
        public void Initialize()
        {
            mockCategoryRepository = new Mock<CategoryRepository>();
            categoryService = new CategoryServiceImpl(mockCategoryRepository.Object);
            fixture = new Fixture(); // Create an instance of Fixture in the setup method or constructor
            categoryDB = fixture.Create<Category>();
        }

        [TestMethod]
        public void AddCategory_ShouldAddCategory_WhenCategoryDoesNotExist()
        {
            // Arrange
            var category = fixture.Create<Category>();
            mockCategoryRepository.Setup(repo => repo.FindByName(category.name)).Returns((Category)null);

            // Act
            categoryService.AddCategory(category);

            // Assert
            mockCategoryRepository.Verify(repo => repo.Add(category), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void AddCategory_ShouldThrowException_WhenCategoryNameIsNull()
        {
            // Arrange
            var category = fixture.Create<Category>();
            category.name = null;

            // Act
            categoryService.AddCategory(category);
        }


        [TestMethod]
        [ExpectedException(typeof(DuplicateNameException))]
        public void AddCategory_ShouldThrowException_WhenCategoryExists()
        {
            // Arrange
            var category = fixture.Create<Category>();
            mockCategoryRepository.Setup(repo => repo.FindByName(category.name)).Returns(categoryDB);

            // Act
            categoryService.AddCategory(category);
        }

        [TestMethod]
        public void FindAllCategories_ShouldReturnListOfCategories()
        {
            // Arrange
            var expectedCategories = fixture.CreateMany<Category>(5).ToList();
            mockCategoryRepository.Setup(repo => repo.FindAll()).Returns(expectedCategories);

            // Act
            var result = categoryService.FindAllCategories();

            // Assert
            CollectionAssert.AreEqual(expectedCategories, result);
        }

        [TestMethod]
        public void UpdateCategory_ShouldUpdateCategory_WhenCategoryExists()
        {
            // Arrange
            var category = fixture.Create<Category>();
            mockCategoryRepository.Setup(repo => repo.FindById(category.id)).Returns(categoryDB);

            // Act
            var result = categoryService.UpdateCategory(category);

            // Assert
            Assert.AreEqual(categoryDB, result);
            mockCategoryRepository.Verify(repo => repo.Update(categoryDB), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void UpdateCategory_ShouldThrowException_WhenCategoryDoesNotExist()
        {
            // Arrange
            var category = fixture.Create<Category>();
            mockCategoryRepository.Setup(repo => repo.FindById(category.id)).Returns((Category)null);

            // Act
            categoryService.UpdateCategory(category);
        }

        [TestMethod]
        public void DeleteCategory_ShouldDeleteCategory_WhenCategoryExistsAndNoAssociatedProducts()
        {
            // Arrange
            var categoryId = 123;
            var categoryDB = fixture.Create<Category>();
            categoryDB.Products = new List<Product>(); // No associated products

            mockCategoryRepository.Setup(repo => repo.FindById(categoryId)).Returns(categoryDB);

            // Act
            categoryService.DeleteCategory(categoryId);

            // Assert
            mockCategoryRepository.Verify(repo => repo.Delete(categoryDB), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void DeleteCategory_ShouldThrowException_WhenCategoryDoesNotExist()
        {
            // Arrange
            var categoryId = 123;

            mockCategoryRepository.Setup(repo => repo.FindById(categoryId)).Returns((Category)null);

            // Act
            categoryService.DeleteCategory(categoryId);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void DeleteCategory_ShouldThrowException_WhenCategoryHasAssociatedProducts()
        {
            // Arrange
            var categoryId = 123;
            var categoryDB = fixture.Create<Category>();
            categoryDB.Products = new List<Product> { fixture.Create<Product>() }; // Associated products

            mockCategoryRepository.Setup(repo => repo.FindById(categoryId)).Returns(categoryDB);

            // Act
            categoryService.DeleteCategory(categoryId);
        }

        [TestMethod]
        public void CountAllCategories_ShouldReturnCorrectCount()
        {
            // Arrange
            var expectedCount = 5; // Change this to the expected count based on your test data
            mockCategoryRepository.Setup(repo => repo.CountAll()).Returns(expectedCount);

            // Act
            var result = categoryService.CountAllCategories();

            // Assert
            Assert.AreEqual(expectedCount, result);
            mockCategoryRepository.Verify(repo => repo.CountAll(), Times.Once);
        }


    }
}
