using Cafe_management_system_backend.MVC.Models;
using System.Collections.Generic;

namespace Cafe_management_system_backend.MVC.Repositories
{
    public interface ProductRepository
    {
        Product FindById(int productId);
        Product FindByName(string productName);
        List<Product> FindAll();
        List<Product> FindAllByCategoryIdAndStatus(int categoryId, string productStatus);
        bool DoesExistById(int productId);
        void Add(Product product);
        void Update(Product product);
        void Delete(Product product);
    }
}
