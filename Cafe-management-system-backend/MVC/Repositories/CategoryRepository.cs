using Cafe_management_system_backend.MVC.Models;
using System.Collections.Generic;

namespace Cafe_management_system_backend.MVC.Repositories
{
    public interface CategoryRepository
    {
        Category FindByName(string categoryName);
        List<Category> FindAll();
        Category FindById(int? categoryId);
        void Add(Category category);
        void Update(Category category);
        void Delete(Category category);
        int CountAll();
    }
}
