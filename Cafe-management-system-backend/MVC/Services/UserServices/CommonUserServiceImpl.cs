using Cafe_management_system_backend.MVC.Models;
using Cafe_management_system_backend.MVC.Repositories;
using NLog;
using System.Collections.Generic;

namespace Cafe_management_system_backend.MVC.Services.UserServices
{
    public class CommonUserServiceImpl : CommonUserService
    {
        private static Logger logger = LogManager.GetLogger("NLogger");
        private readonly UserRepository userRepository;

        public CommonUserServiceImpl(UserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        /// <summary>Retrieves a list of all users (with Role = 'User').</summary>
        /// <returns>A list of User objects representing all users.</returns>
        public List<User> FindAllUsers()
        {
            return userRepository.FindAll();
        }

        /// <summary>Finds a user by their unique identifier (id) and logs the result.</summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>The User object with the specified id, or null if not found.</returns>
        public User FindUserById(int userId)
        {
            User userDB = userRepository.FindById(userId);
            return FindUserWithLogs(userDB);
        }

        /// <summary>Finds a user by their email address and logs the result.</summary>
        /// <param name="userEmail">The email address of the user.</param>
        /// <returns>The User object with the specified email address, or null if not found.</returns>
        public User FindUserByEmail(string userEmail)
        {
            User userDB = userRepository.FindByEmail(userEmail);
            return FindUserWithLogs(userDB);
        }

        /// <summary>Finds a user by their email address and password, and logs the result.</summary>
        /// <param name="userEmail">The email address of the user.</param>
        /// <param name="userPassword">The password of the user.</param>
        /// <returns>The User object with the specified email address and password, or null if not found.</returns>
        public User FindUserByEmailAndPassword(string userEmail, string userPassword)
        {
            User userDB = userRepository.FindByEmailAndPassword(userEmail, userPassword);
            return FindUserWithLogs(userDB);
        }

        /// <summary>Logs information about the success or failure of finding a user.</summary>
        /// <param name="userDB">The User object obtained from the system.</param>
        /// <returns>The User object with additional logging information in the console.</returns>
        private User FindUserWithLogs(User userDB)
        {
            if (userDB != null)
            {
                logger.Info($"[UserService:FindUserWithLogs()] Success: " + "User was successfully found " + "(Id: " + userDB.id + " & Email: " + userDB.email + ")");
                return userDB;
            }
            else
            {
                logger.Info($"[UserService:FindUserWithLogs()] EmptyOrNull: " + "User NOT found...");
                return null;
            }
        }
    }
}