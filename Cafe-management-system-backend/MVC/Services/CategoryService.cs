using Cafe_management_system_backend.MVC.Models;
using System.Collections.Generic;

namespace Cafe_management_system_backend.MVC.Services
{
    public interface CategoryService
    {
        void AddCategory(Category category);
        List<Category> FindAllCategories();
        Category UpdateCategory(Category category);
    }
}
