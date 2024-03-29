﻿using Cafe_management_system_backend.MVC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Cafe_management_system_backend.MVC.Repositories
{
    public class UserRepositoryImpl : ConnectionRepositoryDB, UserRepository
    {
        /// <summary> Finds a user by their unique identifier (id). </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>The user with the specified id, or null if not found.</returns>
        public User FindById(int userId)
        {
            try
            {
                return db.Users.Find(userId);
            }
            catch (Exception ex)
            {
                logger.Error($"[UserRepository:FindById()] Exception: {ex.Message}");
                GetInnerException(ex);
                throw;
            }
        }

        /// <summary> Finds a user by their email address. </summary>
        /// <param name="userEmail">The email address of the user.</param>
        /// <returns>The user with the specified email address, or null if not found.</returns>
        public User FindByEmail(String userEmail)
        {
            try
            {
                return db.Users.Where(u => u.email == userEmail).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error($"[UserRepository:FindByEmail()] Exception: {ex.Message}");
                GetInnerException(ex);
                throw;
            }
        }

        /// <summary>Finds a user by their email address and password.</summary>
        /// <param name="userEmail">The email address of the user.</param>
        /// <param name="userPassword">The password of the user.</param>
        /// <returns>The user with the specified email address and password, or null if not found.</returns>
        public User FindByEmailAndPassword(String userEmail, String userPassword)
        {
            try
            {
                return db.Users.Where(u => (u.email == userEmail && u.password == userPassword)).FirstOrDefault();

            }
            catch (Exception ex)
            {
                logger.Error($"[UserRepository:FindByEmailAndPassword()] Exception: {ex.Message}");
                GetInnerException(ex);
                throw;
            }
        }

        /// <summary>Retrieves all users with a role of "User"</summary>
        /// <returns>A list of users with the specified role.</returns>
        public List<User> FindAll()
        {
            try
            {
                // Step 1: Project to an anonymous type
                // Directly projecting to the User entity type within LINQ to Entities can cause translation problems.
                // Anonymous type helps in performing the initial projection without directly using the User Entity type,
                // addressing the LINQ to Entities query translation issue.
                var usersAnonymous = db.Users
                    .Where(u => u.role == UserRoleEnum.User.ToString())
                    .Select(u => new
                    {
                        u.id,
                        u.name,
                        u.contactNumber,
                        u.email,
                        u.password,
                        u.status,
                        u.role
                    })
                    .ToList();
                // Step 2: Map the anonymous type to the User type
                // After materializing the query, map the results to the User entity type.
                // This ensures that the final result is of the desired User type.
                return usersAnonymous
                    .Select(u => new User
                    {
                        id = u.id,
                        name = u.name,
                        contactNumber = u.contactNumber,
                        email = u.email,
                        password = u.password,
                        status = u.status,
                        role = u.role
                    })
                    .ToList();
            } catch(Exception ex)
            {
                logger.Error($"[UserRepository:FindAll()] Exception: {ex.Message}");
                GetInnerException(ex);
                throw;
            }
        }

        /// <summary>Adds a new user to the system.</summary>
        /// <param name="user">The user to be added.</param>
        public void Add(User user)
        {
            try
            {
                db.Users.Add(user);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error($"[UserRepository:Add()] Exception: {ex.Message}");
                GetInnerException(ex);
                throw;
            }
        }

        /// <summary>Updates an existing user in the system.</summary>
        /// <param name="user">The user to be updated.</param>
        public void Update(User user)
        {
            try
            {
                // User Entity is considered as Modified form
                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                // Save Updated user
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error($"[UserRepository:Update()] Exception: {ex.Message}");
                GetInnerException(ex);
                throw;
            }
        }

        /// <summary> Deletes a user from the system. </summary>
        /// <param name="user"> The User object to be deleted. </param>
        /// <exception cref="Exception"> Thrown when an error occurs during the delete operation. </exception>
        public void Delete(User user)
        {
            try
            {
                db.Users.Attach(user); // Inform Entity Framework that User Entity is now being tracked/considered for operation
                db.Users.Remove(user);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error($"[UserRepository:Delete()] Exception: {ex.Message}");
                GetInnerException(ex);
                throw;
            }
        }

        /// <summary>Counts the total number of users.</summary>
        /// <returns>The total number of users in the database.</returns>
        public int CountAll()
        {
            try
            {
                return db.Users.Count();
            }
            catch (Exception ex)
            {
                logger.Error($"[UserRepository:CountAll()] Exception: {ex.Message}");
                GetInnerException(ex);
                throw;
            }
        }
    }
}