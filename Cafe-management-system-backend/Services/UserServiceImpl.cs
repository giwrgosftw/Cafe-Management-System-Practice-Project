using Cafe_management_system_backend.Models;
using Cafe_management_system_backend.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace Cafe_management_system_backend.Services
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
            User userObjDB = userRepository.FindByEmailAndPassword(user.email, user.password);
            if (userObjDB != null)
            {
                if (userObjDB.status == "true")
                {
                    return new {token = TokenManager.GenerateToken(userObjDB.email, userObjDB.password) };
                }
                else
                {
                    throw new UnauthorizedAccessException("Wait for Admin Approval");
                }
            }
            else
            {
                throw new UnauthorizedAccessException("Incorrect Username or Password");
            }
        }
    }
}