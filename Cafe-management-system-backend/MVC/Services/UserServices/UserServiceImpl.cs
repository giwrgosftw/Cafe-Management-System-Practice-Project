using Cafe_management_system_backend.MVC.Models;
using Cafe_management_system_backend.MVC.Repositories;
using Cafe_management_system_backend.MVC.Security;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;

namespace Cafe_management_system_backend.MVC.Services.UserServices
{
    public class UserServiceImpl : UserService
    {
        private static Logger logger = LogManager.GetLogger("NLogger");
        private readonly UserRepository userRepository;
        private readonly CommonUserService commonUserService;

        // A constructor to initialize userRepository
        public UserServiceImpl(UserRepository userRepository, CommonUserService commonUserService)
        {
            this.userRepository = userRepository;
            this.commonUserService = commonUserService;
        }

        /// <summary>This method registers a new user to the system.</summary>
        /// <param name="user">The User object containing user information.</param>
        public void SignUp(User user)
        {
            if(user.email == null)
            {
                logger.Error("[UserService:SignUp()] Exception: User email NOT given.");
                throw new ArgumentException("User email NOT given.");
            }
            User userDB = userRepository.FindByEmail(user.email);
            if (userDB == null)
            {
                // If e-mail not exist (since new), setup the appropriate User values
                user.role = UserRoleEnum.User.ToString(); // by default is a user
                user.status = "false"; // never logged-in before
                userRepository.Add(user); // Add to the database
                logger.Info("[UserService:SignUp() Success]: User (Email: {UserEmail}) was created successfully!", user.email);
            }
            else
            {
                // Return an error message
                logger.Error("[UserService:SignUp()] Exception: User already exists (Email: {UserEmail})", user.email);
                throw new DuplicateNameException("Email already exists. Please use a different email address.");
            }
        }

        /// <summary>This method login a user and generates an authentication token.</summary>
        /// <param name="user">The User object containing login credentials.</param>
        /// <returns>An object containing the generated token.</returns>
        public object Login(User user)
        {
            // Retrieve user information from the database based on email and password
            User userDB = userRepository.FindByEmailAndPassword(user.email, user.password);
            // Check if a user with the given email and password was found
            if (userDB != null)
            {
                // Check if the user account is active ("true")
                if (userDB.status == "true")
                {
                    // Generate a token for the authenticated user
                    var token = TokenManager.GenerateToken(userDB.email, userDB.role);
                    logger.Info("[UserService:Login() Success]: User's (Email: {UserEmail}) token was generated successfully! \n{Token}", userDB.email, token);
                    return new {token};
                }
                else
                {
                    // User account is not active, throw an exception indicating that admin approval is required
                    logger.Error("[UserService:Login()] Exception: User has status '{UserStatus}' (Email: {UserEmail})", userDB.status, userDB.email);
                    throw new UnauthorizedAccessException("Wait for Admin Approval");
                }
            }
            else
            {
                // User with the provided email and password not found, throw an exception indicating incorrect credentials
                logger.Error("[UserService:Login()] Exception: Incorrect Username or Password (Email: {UserEmail})", user.email);
                throw new UnauthorizedAccessException("Incorrect Username or Password");
            }
        }

        /// <summary>This method updates user information.</summary>
        /// <param name="user">The User object containing updated information.</param>
        /// <returns>The updated User object.</returns>
        public User UpdateUser(User user)
        {
            User userDB = UpdateUserEntity(user); // update hard-coded
            userRepository.Update(userDB);  // update/save into DB
            logger.Info("[UserService:Update()] Success: User updated successfully (Id: {UserId} & Email: {UserEmail})", userDB.id, userDB.email);
            return userDB;
        }

        /// <summary>This method changes the user's password.</summary>
        /// <param name="principal">The PrincipalProfile object containing user information.</param>
        /// <param name="changePassword">The ChangePassword object containing old and new passwords.</param>
        /// <returns>The updated User object.</returns>
        public User ChangeUserPassword(PrincipalProfile principal, ChangePassword changePassword)
        {
            // Check first if a New Password was given indeed
            if(changePassword.newPassword == null)
            {
                logger.Error("[UserService:ChangeUserPassword()] Exception: New Password NOT given (Email: {UserEmail})", principal.Email);
                throw new ArgumentException($"New Password NOT given (Email: {principal.Email})");
            }
            // Find user by Email and Password while finding/checking if null
            User userDB = commonUserService.FindUserByEmailAndPassword(principal.Email, changePassword.oldPassword);
            // Update password
            if(userDB != null) { 
                userDB.password = changePassword.newPassword ?? userDB.password;
                userRepository.Update(userDB);
            } 
            else
            {
                logger.Error("[UserService:ChangeUserPassword()] Exception: Incorrect Old Password (Email: {UserEmail})", principal.Email);
                throw new InvalidOperationException($"Incorrect Old Password (Email: {principal.Email})");
            }
            return userDB;
        }

        /// <summary> Deletes a user from the system, ensuring that an admin cannot delete another admin. </summary>
        /// <param name="userId"> The ID of the user to be deleted. </param>
        /// <exception cref="KeyNotFoundException"> Thrown when the specified user ID is not found. </exception>
        /// <exception cref="UnauthorizedAccessException"> Thrown when an attempt is made to delete an admin by another admin. </exception>
        public void DeleteUser(int userId)
        {
            User userDB = commonUserService.FindUserById(userId);
            if (userDB == null)
            {
                logger.Error("[UserService:DeleteUser()] Failed: User with given Id NOT found (Id: {UserId})", userId);
                throw new KeyNotFoundException($"User with given Id NOT found (Id: {userId})");
            }
            // An Admin cannot delete other admins, only Users
            if (userDB.role == UserRoleEnum.Admin.ToString())
            {
                logger.Error("[UserService:DeleteUser()] Failed: An Admin cannot delete another Admin");
                throw new UnauthorizedAccessException();
            }
            userRepository.Delete(userDB);
        }

        /// <summary> Removes the account of the authenticated user based on the provided principal information. </summary>
        /// <param name="principal"> The PrincipalProfile containing user authentication information. </param>
        /// <param name="userId"> The ID of the logged-in user to be deleted. </param>
        /// <exception cref="KeyNotFoundException"> Thrown when the specified user ID is not found. </exception>
        public void DeleteMyAccount(string principalEmail)
        {
            User userDB = commonUserService.FindUserByEmail(principalEmail);
            if (userDB == null)
            {
                logger.Error("[UserService:DeleteMyAccount()] Failed: Principal with given Email NOT found (Id: {PrincipalEmail})", principalEmail);
                throw new KeyNotFoundException($"Principal with given Email NOT found (Email: {principalEmail})");
            }
            userRepository.Delete(userDB);
        }

        /// <summary>Counts the total number of categories in the database.</summary>
        /// <returns>The total number of categories.</returns>
        public int CountAllUsers()
        {
            return userRepository.CountAll();
        }

        /// <summary>Updates the User entity with the provided user information.</summary>
        /// <param name="user">The User object containing updated information.</param>
        /// <returns>The updated User object.</returns>
        /* TODO: this method was implemented for learning purposes, we do not need this extra method, instead,
         * in the Repository class, update using 'db.Entry(objectEntity).CurrentValues.SetValues(updatedObjectEntity);'
        */
        private User UpdateUserEntity(User user)
        {
            User userDB = commonUserService.FindUserById(user.id);
            if (userDB != null)
            {
                userDB.name = user.name ?? userDB.name; // if userDB.name = NULL do not change it
                userDB.contactNumber = user.contactNumber ?? userDB.contactNumber;
                userDB.email = user.email ?? userDB.email;
                userDB.password = user.password ?? userDB.password;
                userDB.status = user.status ?? userDB.status;
                userDB.role = user.role ?? userDB.role;
                return userDB;
            }
            else
            {
                logger.Error("[UserService:Update()] Failed: User with given Id NOT found (Id: {UserId})", user.id);
                throw new KeyNotFoundException($"User with given Id NOT found (Id: {user.id})");
            }
        }

    }
}