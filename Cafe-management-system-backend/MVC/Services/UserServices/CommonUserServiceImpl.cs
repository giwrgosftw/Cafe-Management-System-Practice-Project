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

        public List<User> FindAllUsers()
        {
            return userRepository.FindAll();
        }

        public User FindUserById(int userId)
        {
            User userDB = userRepository.FindById(userId);
            return FindUserWithLogs(userDB);
        }

        public User FindUserByEmail(string userEmail)
        {
            User userDB = userRepository.FindByEmail(userEmail);
            return FindUserWithLogs(userDB);
        }

        public User FindUserByEmailAndPassword(string userEmail, string userPassword)
        {
            User userDB = userRepository.FindByEmailAndPassword(userEmail, userPassword);
            return FindUserWithLogs(userDB);
        }

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