using Cafe_management_system_backend.MVC.Models;
using System.Collections.Generic;

namespace Cafe_management_system_backend.MVC.Services
{
    public interface ProductService
    {
        void AddProduct(Product product);
        Product UpdateProduct(Product product);
        void DeleteProduct(int productId);
        Product FindProductById(int productId);
        List<Product> FindAllProducts();
        List<Product> FindProductsByCategoryIdAndStatus(int categoryId, string productStatus);
        bool DoesProductExistById(int productId);
        int CountAllProducts();
    }
}
