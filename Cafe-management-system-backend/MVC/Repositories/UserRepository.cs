﻿using Cafe_management_system_backend.MVC.Models;
using System;
using System.Collections.Generic;

namespace Cafe_management_system_backend.MVC.Repositories
{
    public interface UserRepository
    {
        User FindById(int userId);
        User FindByEmail(String userEmail);
        User FindByEmailAndPassword(String userEmail, String userPassword);
        List<User> FindAll();
        void AddUser(User user);
        void UpdateUser(User user);
    }
}
