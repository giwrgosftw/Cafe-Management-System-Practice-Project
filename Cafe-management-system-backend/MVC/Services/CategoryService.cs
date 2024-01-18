using Cafe_management_system_backend.MVC.Models;
using System.Collections.Generic;

namespace Cafe_management_system_backend.MVC.Services
{
    public interface CategoryService
    {
        List<Category> FindAllCategories();
        Category FindCategoryByIdWithoutException(int? categoryId);
        Category FindCategoryByIdWithException(int? categoryId);
        void AddCategory(Category category);
        Category UpdateCategory(Category category);
        void DeleteCategory(int categoryId);
        int CountAllCategories();
    }
}
