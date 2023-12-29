using Cafe_management_system_backend.MVC.Models;
using Cafe_management_system_backend.MVC.Repositories;
using Cafe_management_system_backend.MVC.Services.UserServices;
using Moq;
using AutoFixture;

namespace Cafe_management_system_backend_tests.UserServices
{

    [TestClass]
    public class CommonUserServiceImplTests
    {
        private Mock<UserRepository> mockUserRepository;
        private CommonUserServiceImpl commonUserServiceImpl;
        private Fixture fixture;
        private User userDB;

        [TestInitialize]
        public void Initialize()
        {
            mockUserRepository = new Mock<UserRepository>();
            commonUserServiceImpl = new CommonUserServiceImpl(mockUserRepository.Object);
            fixture = new Fixture(); // Create an instance of Fixture in the setup method or constructor
            userDB = fixture.Create<User>();
        }

        [TestMethod]
        public void FindAllUsers_ShouldReturnListOfUsers()
        {
            // Arrange/Given
            var expectedUsers = fixture.CreateMany<User>(5).ToList();
            mockUserRepository.Setup(repo => repo.FindAll()).Returns(expectedUsers);
            // Act/When
            var result = commonUserServiceImpl.FindAllUsers();
            // Assert/Then
            CollectionAssert.AreEqual(expectedUsers, result);
        }

        [TestMethod]
        public void FindUserById_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            mockUserRepository.Setup(repo => repo.FindById(userDB.id)).Returns(userDB);
            // Act
            var result = commonUserServiceImpl.FindUserById(userDB.id);
            // Assert
            Assert.AreEqual(userDB, result);
            //mockLogger.Verify(logger => logger.Info($"[UserService:FindUserWithLogs()] Success: User was successfully found (Id: {userDB.id} & Email: {userDB.email})"), Times.Once);
        }

        [TestMethod]
        public void FindUserById_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            int userId = 0;
            mockUserRepository.Setup(repo => repo.FindById(userId)).Returns((User)null);
            // Act
            var result = commonUserServiceImpl.FindUserById(userId);
            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void FindUserByEmail_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            mockUserRepository.Setup(repo => repo.FindByEmail(userDB.email)).Returns(userDB);
            // Act
            var result = commonUserServiceImpl.FindUserByEmail(userDB.email);
            // Assert
            Assert.AreEqual(userDB, result);
        }

        [TestMethod]
        public void FindUserByEmail_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var userEmail = "nonexistent@example.com";
            mockUserRepository.Setup(repo => repo.FindByEmail(userEmail)).Returns((User)null);
            // Act
            var result = commonUserServiceImpl.FindUserByEmail(userEmail);
            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void FindUserByEmailAndPassword_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            mockUserRepository.Setup(repo => repo.FindByEmailAndPassword(userDB.email, userDB.password)).Returns(userDB);
            // Act
            var result = commonUserServiceImpl.FindUserByEmailAndPassword(userDB.email, userDB.password);
            // Assert
            Assert.AreEqual(userDB, result);
        }

        [TestMethod]
        public void FindUserByEmailAndPassword_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var userEmail = "nonexistent@example.com";
            var userPassword = "password";
            mockUserRepository.Setup(repo => repo.FindByEmailAndPassword(userEmail, userPassword)).Returns((User)null);
            // Act
            var result = commonUserServiceImpl.FindUserByEmailAndPassword(userEmail, userPassword);
            // Assert
            Assert.IsNull(result);
        }
    }

}