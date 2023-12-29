using Cafe_management_system_backend.MVC.Models;
using System.Collections.Generic;

namespace Cafe_management_system_backend.MVC.Repositories
{
    public interface CategoryRepository
    {
        Category FindByName(string categoryName);
        void AddCategory(Category category);
        List<Category> FindAll();
        Category FindById(int categoryId);
        void Update(Category category);
    }
}
