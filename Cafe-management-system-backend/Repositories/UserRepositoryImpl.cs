using Cafe_management_system_backend.Models;
using System;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace Cafe_management_system_backend.Repositories
{
    public class UserRepositoryImpl : ConnectionRepositoryDB, UserRepository
    {
        public void AddUser(User user)
        {
            try
            {
                db.Users.Add(user);
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                // TODO: For production, use a Logger library/framework instead
                Console.WriteLine($"Error adding user to the database: {ex.Message}");
                // Rethrow the exception or handle it as needed
                throw;
            }
        }

        public User FindByEmail(String userEmail)
        {
            return db.Users.Where(u => u.email == userEmail).FirstOrDefault();
        }

        public User FindByEmailAndPassword(String userEmail, String userPassword)
        {
            return db.Users.Where(u => (u.email == userEmail && u.password == userPassword)).FirstOrDefault(); 
        }
    }
}