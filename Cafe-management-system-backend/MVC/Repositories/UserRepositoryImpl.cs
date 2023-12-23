using Cafe_management_system_backend.MVC.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace Cafe_management_system_backend.MVC.Repositories
{
    public class UserRepositoryImpl : ConnectionRepositoryDB, UserRepository
    {
        public User FindById(int userId)
        {
            try
            {
                return db.Users.Find(userId);
            }
            catch (Exception ex)
            {
                logger.Error($"[UserRepository:FindById()] Exception: {ex.Message}");
                GetInnerException(ex, "FindById");
                throw;
            }
        }

        public User FindByEmail(String userEmail)
        {
            try
            {
                return db.Users.Where(u => u.email == userEmail).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error($"[UserRepository:FindByEmail()] Exception: {ex.Message}");
                GetInnerException(ex, "FindByEmail()");
                throw;
            }
        }

        public User FindByEmailAndPassword(String userEmail, String userPassword)
        {
            try
            {
                return db.Users.Where(u => (u.email == userEmail && u.password == userPassword)).FirstOrDefault();

            }
            catch (Exception ex)
            {
                logger.Error($"[UserRepository:FindByEmailAndPassword()] Exception: {ex.Message}");
                GetInnerException(ex, "FindByEmailAndPassword()");
                throw;
            }
        }

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
                GetInnerException(ex, "FindAll()");
                throw;
            }
        }

        public void AddUser(User user)
        {
            try
            {
                db.Users.Add(user);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error($"[UserRepository:AddUser()] Exception: {ex.Message}");
                GetInnerException(ex, "AddUser()");
                throw;
            }
        }

        public void UpdateUser(User user)
        {
            try
            {
                // User Entity is considered as Modified form
                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                // Save Updated user
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                logger.Error($"[UserRepository:UpdateUser()] Exception: {ex.Message}");
                GetInnerException(ex, "UpdateUser()");
                throw;
            }
        }

    }
}