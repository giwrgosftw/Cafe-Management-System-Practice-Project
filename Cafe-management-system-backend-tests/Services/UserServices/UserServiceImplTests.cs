using Moq;
using AutoFixture;
using Cafe_management_system_backend.MVC.Models;
using Cafe_management_system_backend.MVC.Repositories;
using Cafe_management_system_backend.MVC.Security;
using Cafe_management_system_backend.MVC.Services.UserServices;
using System.Data;

namespace Cafe_management_system_backend_tests.UserServices
{
    [TestClass]
    public class UserServiceImplTests
    {
        private Mock<UserRepository> mockUserRepository;
        private Mock<CommonUserService> mockCommonUserService;
        private UserServiceImpl userServiceImpl;
        private Fixture fixture;

        [TestInitialize]
        public void Initialize()
        {
            mockUserRepository = new Mock<UserRepository>();
            mockCommonUserService = new Mock<CommonUserService>();
            userServiceImpl = new UserServiceImpl(mockUserRepository.Object, mockCommonUserService.Object);
            fixture = new Fixture();
        }

        [TestMethod]
        public void SignUp_ShouldCreateUser_WhenEmailDoesNotExist()
        {
            // Arrange
            var newUser = fixture.Create<User>();
            newUser.email = "newuser@example.com";

            mockUserRepository.Setup(repo => repo.FindByEmail(newUser.email)).Returns((User)null);

            // Act
            userServiceImpl.SignUp(newUser);

            // Assert
            mockUserRepository.Verify(repo => repo.Add(newUser), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(DuplicateNameException))]
        public void SignUp_ShouldThrowException_WhenEmailExists()
        {
            // Arrange
            var existingUser = fixture.Create<User>();
            existingUser.email = "existinguser@example.com";

            mockUserRepository.Setup(repo => repo.FindByEmail(existingUser.email)).Returns(existingUser);

            // Act
            userServiceImpl.SignUp(existingUser);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void SignUp_ShouldThrowException_WhenUserEmailIsNull()
        {
            // Arrange
            var userWithNullEmail = fixture.Create<User>();
            userWithNullEmail.email = null;

            // Act
            userServiceImpl.SignUp(userWithNullEmail);

            // Assert
            // The ExpectedException attribute is used to specify that the test method
            // is expected to throw the specified exception type (Exception) during its execution.
        }


        [TestMethod]
        public void Login_ShouldGenerateToken_WhenCredentialsAreCorrect()
        {
            // Arrange
            var loginUser = fixture.Create<User>();
            loginUser.email = "user@example.com";
            loginUser.password = "password";

            var userDB = fixture.Create<User>();
            userDB.status = "true"; // Active user

            // Set up the UserRepository mock
            mockUserRepository.Setup(repo => repo.FindByEmailAndPassword(loginUser.email, loginUser.password)).Returns(userDB);

            // Set up the CommonUserService mock
            mockCommonUserService.Setup(service => service.FindUserByEmailAndPassword(loginUser.email, loginUser.password)).Returns(userDB);

            // Act
            var result = userServiceImpl.Login(loginUser);

            // Assert
            Assert.IsNotNull(result);

            // Verify that the UserRepository mock was called
            mockUserRepository.Verify(repo => repo.FindByEmailAndPassword(loginUser.email, loginUser.password), Times.Once);

            // Verify that the CommonUserService mock was called
            //mockCommonUserService.Verify(service => service.FindUserByEmailAndPassword(loginUser.email, loginUser.password), Times.Once);
        }


        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void Login_ShouldThrowUnauthorizedException_WhenUserStatusIsInactive()
        {
            // Arrange
            var inactiveUser = fixture.Create<User>();
            inactiveUser.status = "false"; // Inactive user

            mockUserRepository.Setup(repo => repo.FindByEmailAndPassword(inactiveUser.email, inactiveUser.password)).Returns(inactiveUser);

            // Act
            userServiceImpl.Login(inactiveUser);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void Login_ShouldThrowUnauthorizedException_WhenUserNotFound()
        {
            // Arrange
            var loginUser = fixture.Create<User>();
            loginUser.email = "nonexistent@example.com";

            mockUserRepository.Setup(repo => repo.FindByEmailAndPassword(loginUser.email, loginUser.password)).Returns((User)null);

            // Act
            userServiceImpl.Login(loginUser);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void Login_ShouldThrowUnauthorizedException_WhenUserStatusIsPending()
        {
            // Arrange
            var pendingUser = fixture.Create<User>();
            pendingUser.status = "pending"; // Pending user

            mockUserRepository.Setup(repo => repo.FindByEmailAndPassword(pendingUser.email, pendingUser.password)).Returns(pendingUser);

            // Act
            userServiceImpl.Login(pendingUser);
        }

        [TestMethod]
        public void UpdateUser_ShouldUpdateUser_WhenUserExists()
        {
            // Arrange
            var updateUser = fixture.Create<User>();
            var updatedUserDB = fixture.Create<User>();

            mockCommonUserService.Setup(service => service.FindUserById(updateUser.id)).Returns(updatedUserDB);

            // Act
            var result = userServiceImpl.UpdateUser(updateUser);

            // Assert
            Assert.AreEqual(updatedUserDB, result);
            mockUserRepository.Verify(repo => repo.Update(updatedUserDB), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void UpdateUser_ShouldThrowException_WhenUserDoesNotExist()
        {
            // Arrange
            var updateUser = fixture.Create<User>();

            mockCommonUserService.Setup(service => service.FindUserById(updateUser.id)).Returns((User)null);

            // Act
            userServiceImpl.UpdateUser(updateUser);
        }

        [TestMethod]
        public void ChangeUserPassword_ShouldChangePassword_WhenOldPasswordIsCorrect()
        {
            // Arrange
            var principal = fixture.Create<PrincipalProfile>();
            var changePassword = fixture.Create<ChangePassword>();
            var userDB = fixture.Create<User>();

            mockCommonUserService.Setup(service => service.FindUserByEmailAndPassword(principal.Email, changePassword.oldPassword)).Returns(userDB);

            // Act
            var result = userServiceImpl.ChangeUserPassword(principal, changePassword);

            // Assert
            Assert.AreEqual(userDB, result);
            mockUserRepository.Verify(repo => repo.Update(userDB), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ChangeUserPassword_ShouldThrowException_WhenOldPasswordIsIncorrect()
        {
            // Arrange
            var principal = fixture.Create<PrincipalProfile>();
            var changePassword = fixture.Create<ChangePassword>();

            mockCommonUserService.Setup(service => service.FindUserByEmailAndPassword(principal.Email, changePassword.oldPassword)).Returns((User)null);

            // Act
            userServiceImpl.ChangeUserPassword(principal, changePassword);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ChangeUserPassword_ShouldThrowException_WhenNewPasswordIsNull()
        {
            // Arrange
            var principal = fixture.Create<PrincipalProfile>();
            var changePassword = fixture.Create<ChangePassword>();
            var userDB = fixture.Create<User>();

            mockCommonUserService.Setup(service => service.FindUserByEmailAndPassword(principal.Email, changePassword.oldPassword)).Returns(userDB);

            // Act
            userServiceImpl.ChangeUserPassword(principal, new ChangePassword { oldPassword = "old", newPassword = null });
        }

        [TestMethod]
        public void DeleteUser_ShouldDeleteUser_WhenUserExistsAndNotAdmin()
        {
            // Arrange
            var userId = 123;
            var userDB = fixture.Create<User>();
            userDB.role = UserRoleEnum.User.ToString();

            mockCommonUserService.Setup(service => service.FindUserById(userId)).Returns(userDB);

            // Act
            userServiceImpl.DeleteUser(userId);

            // Assert
            mockUserRepository.Verify(repo => repo.Delete(userDB), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void DeleteUser_ShouldThrowException_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 123;

            mockCommonUserService.Setup(service => service.FindUserById(userId)).Returns((User)null);

            // Act
            userServiceImpl.DeleteUser(userId);
        }

        [TestMethod]
        [ExpectedException(typeof(UnauthorizedAccessException))]
        public void DeleteUser_ShouldThrowException_WhenDeletingAdminByAnotherAdmin()
        {
            // Arrange
            var adminId = 123;
            var adminDB = fixture.Create<User>();
            adminDB.role = UserRoleEnum.Admin.ToString();

            mockCommonUserService.Setup(service => service.FindUserById(adminId)).Returns(adminDB);

            // Act
            userServiceImpl.DeleteUser(adminId);
        }

        [TestMethod]
        public void DeleteMyAccount_ShouldDeleteAccount_WhenUserExists()
        {
            // Arrange
            var principalEmail = "user@example.com";
            var userDB = fixture.Create<User>();

            mockCommonUserService.Setup(service => service.FindUserByEmail(principalEmail)).Returns(userDB);

            // Act
            userServiceImpl.DeleteMyAccount(principalEmail);

            // Assert
            mockUserRepository.Verify(repo => repo.Delete(userDB), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void DeleteMyAccount_ShouldThrowException_WhenUserDoesNotExist()
        {
            // Arrange
            var principalEmail = "nonexistent@example.com";

            mockCommonUserService.Setup(service => service.FindUserByEmail(principalEmail)).Returns((User)null);

            // Act
            userServiceImpl.DeleteMyAccount(principalEmail);
        }

    }
}
