using Cafe_management_system_backend.MVC.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Cafe_management_system_backend.MVC.Repositories
{
    public class UserRepositoryImpl : ConnectionRepositoryDB, UserRepository
    {
        private static Logger logger = LogManager.GetLogger("NLogger");

        public User FindByEmail(String userEmail)
        {
            try
            {
                return db.Users.Where(u => u.email == userEmail).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error($"[Repo-Method: FindByEmail()] Exception: {ex.Message}");
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
                logger.Error($"[Repo-Method: FindByEmailAndPassword()] Exception: {ex.Message}");
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
                logger.Error($"[Repo-Method: AddUser()] Exception: {ex.Message}");
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
                logger.Error($"[Repo-Method: FindAll()] Exception: {ex.Message}");
                throw;
            }
        }

    }
}