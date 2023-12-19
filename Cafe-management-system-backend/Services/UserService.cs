using Cafe_management_system_backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe_management_system_backend.Services
{
    public interface UserService
    {
        void SignUp(User user);
        object Login(User user);
    }
}
