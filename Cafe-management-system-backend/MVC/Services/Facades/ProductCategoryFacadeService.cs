using Cafe_management_system_backend.MVC.Models;
using System.Collections.Generic;

namespace Cafe_management_system_backend.MVC.Services.Facades
{
    public interface ProductCategoryFacadeService
    {
        void AddProductWithCategory(Product product);
        List<Product> GetProductsByCategoryIdAndStatus(int categoryId, string productStatus);
    }
}
