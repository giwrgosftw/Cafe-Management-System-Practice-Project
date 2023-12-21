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

        // Add a constructor to initialize userRepository
        public UserServiceImpl(UserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public void SignUp(User user)
        {
            User userObjDB = userRepository.FindByEmail(user.email);
            if (userObjDB == null)
            {
                // If e-mail not exist (since new), setup the appropriate User values
                user.role = "user"; // by default is a user
                user.status = "false"; // never logged-in before
                userRepository.AddUser(user); // Add to the database
                logger.Info($"[Service-Method: SignUp() Success]: User (Email: " + user.email + ") was created successfully!");
            }
            else
            {
                // Return an error message
                logger.Error($"[Service-Method: SignUp()] Exception: " + "User already exists " + "(Email: " + user.email + ")");
                throw new DuplicateNameException("Email already exists. Please use a different email address.");
            }
        }

        public object Login(User user)
        {
            // Retrieve user information from the database based on email and password
            User userObjDB = userRepository.FindByEmailAndPassword(user.email, user.password);
            // Check if a user with the given email and password was found
            if (userObjDB != null)
            {
                // Check if the user account is active ("true")
                if (userObjDB.status == "true")
                {
                    // Generate a token for the authenticated user
                    var token = TokenManager.GenerateToken(userObjDB.email, userObjDB.role);
                    logger.Info($"[Service-Method: Login() Success]: User's (Email: " + userObjDB.email + ") token was generated successfully! \n" + token);
                    return new {token};
                }
                else
                {
                    // User account is not active, throw an exception indicating that admin approval is required
                    logger.Error($"[Service-Method: Login()] Exception: " + "User has status '" + userObjDB.status + "' " + "(Email: " + userObjDB.email + ")");
                    throw new UnauthorizedAccessException("Wait for Admin Approval");
                }
            }
            else
            {
                // User with the provided email and password not found, throw an exception indicating incorrect credentials
                logger.Error($"[Service-Method: Login()] Exception: Incorrect Username or Password " + "(Email: " + user.email + ")" );
                throw new UnauthorizedAccessException("Incorrect Username or Password");
            }
        }

        public List<User> FindAllUsers()
        {
            return userRepository.FindAll();
        }

    }
}