﻿using Cafe_management_system_backend.MVC.Models;
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

        // Add a constructor to initialize userRepository
        public UserServiceImpl(UserRepository userRepository, CommonUserService commonUserService)
        {
            this.userRepository = userRepository;
            this.commonUserService = commonUserService;
        }

        public void SignUp(User user)
        {
            if(user.email == null)
            {
                logger.Error("[UserService:SignUp()] Exception: User email NOT given.");
                throw new Exception();
            }
            User userDB = userRepository.FindByEmail(user.email);
            if (userDB == null)
            {
                // If e-mail not exist (since new), setup the appropriate User values
                user.role = "user"; // by default is a user
                user.status = "false"; // never logged-in before
                userRepository.AddUser(user); // Add to the database
                logger.Info("[UserService:SignUp() Success]: User (Email: {UserEmail}) was created successfully!", user.email);
            }
            else
            {
                // Return an error message
                logger.Error("[UserService:SignUp()] Exception: User already exists (Email: {UserEmail})", user.email);
                throw new DuplicateNameException("Email already exists. Please use a different email address.");
            }
        }

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

        public User UpdateUser(User user)
        {
            User userDB = UpdateUserEntity(user); // update hard-coded
            userRepository.UpdateUser(userDB);  // update/save into DB
            logger.Info("[UserService:Update()] Success: User updated successfully (Id: {UserId} & Email: {UserEmail})", userDB.id, userDB.email);
            return userDB;
        }

        public User ChangeUserPassword(PrincipalProfile principal, ChangePassword changePassword)
        {
            // Check first if a New Password was given indeed
            if(changePassword.newPassword == null)
            {
                logger.Error("[UserService:ChangeUserPassword()] Exception: New Password NOT given (Email: {UserEmail})", principal.Email);
                throw new Exception();
            }
            // Find user by Email and Password while finding/checking if null
            User userDB = commonUserService.FindUserByEmailAndPassword(principal.Email, changePassword.oldPassword);
            // Update password
            if(userDB != null) { 
                userDB.password = changePassword.newPassword ?? userDB.password;
                userRepository.UpdateUser(userDB);
            } 
            else
            {
                logger.Error("[UserService:ChangeUserPassword()] Exception: Incorrect Old Password (Email: {UserEmail})", principal.Email);
                throw new InvalidOperationException();
            }
            return userDB;
        }

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
                throw new KeyNotFoundException();
            }
        }

    }
}