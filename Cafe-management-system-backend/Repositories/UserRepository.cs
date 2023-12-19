using Cafe_management_system_backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe_management_system_backend.Repositories
{
    public interface UserRepository
    {
        void AddUser(User user);
        User FindByEmail(String userEmail);
        User FindByEmailAndPassword(String userEmail, String userPassword);
        
    }
}
