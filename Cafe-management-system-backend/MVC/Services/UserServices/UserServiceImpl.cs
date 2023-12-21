using Cafe_management_system_backend.MVC.Models;
using Cafe_management_system_backend.MVC.Repositories;
using Cafe_management_system_backend.MVC.Security;
using System;
using System.Collections.Generic;

namespace Cafe_management_system_backend.MVC.Services.UserServices
{
    public class UserServiceImpl : UserService
    {
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
                // Add to the database
                userRepository.AddUser(user);
            }
            else
            {
                // Return an error message
                throw new ApplicationException("Email already exists. Please use a different email address.");
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
                    return new {token = TokenManager.GenerateToken(userObjDB.email, userObjDB.role) };
                }
                else
                {
                    // User account is not active, throw an exception indicating that admin approval is required
                    throw new UnauthorizedAccessException("Wait for Admin Approval");
                }
            }
            else
            {
                // User with the provided email and password not found, throw an exception indicating incorrect credentials
                throw new UnauthorizedAccessException("Incorrect Username or Password");
            }
        }

        public List<User> FindAllUsers()
        {
            return userRepository.FindAll();
        }

    }
}