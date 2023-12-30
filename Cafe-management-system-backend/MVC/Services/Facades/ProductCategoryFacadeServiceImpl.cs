using Cafe_management_system_backend.MVC.Models;
using NLog;
using System.Collections.Generic;

namespace Cafe_management_system_backend.MVC.Services.Facades
{
    public class ProductCategoryFacadeServiceImpl: ProductCategoryFacadeService
    {
        private static Logger logger = LogManager.GetLogger("NLogger");
        private readonly CategoryService categoryService;
        private readonly ProductService productService;

        public ProductCategoryFacadeServiceImpl(CategoryService categoryService, ProductService productService)
        {
            this.categoryService = categoryService;
            this.productService = productService;
        }

        /// <summary> Adds a new product to the system with a specified category, ensuring that the category exists. </summary>
        /// <param name="product"> The Product object to be added. </param>
        /// <exception cref="KeyNotFoundException"> Thrown when the specified category ID is not found. </exception>
        public void AddProductWithCategory(Product product)
        {
            // Ensure that the Category exists
            Category categoryDB = categoryService.FindCategoryById(product.categoryId);
            if (categoryDB == null)
            {
                logger.Error("[ProductCategoryFacadeService:AddProductWithCategory()] Exception: Category with given Id NOT found (Id: {CategoryId}).", product.categoryId);
                throw new KeyNotFoundException();
            }
            // Add the new Product
            productService.AddProduct(product);
        }

        /// <summary> Retrieves a list of products in the system based on category ID and status, ensuring that the category exists. </summary>
        /// <param name="categoryId"> The ID of the category. </param>
        /// <param name="productStatus"> The status of the products to be retrieved. </param>
        /// <returns> A list of Product objects matching the specified category ID and status. </returns>
        /// <exception cref="KeyNotFoundException"> Thrown when the specified category ID is not found. </exception>
        public List<Product> GetProductsByCategoryIdAndStatus(int categoryId, string productStatus)
        {
            // Ensure that the Category exists
            Category categoryDB = categoryService.FindCategoryById(categoryId);
            if (categoryDB == null)
            {
                logger.Error("[ProductCategoryFacadeService:GetProductsByCategoryIdAndStatus()] Exception: Category with given Id NOT found (Id: {CategoryId}).", categoryId);
                throw new KeyNotFoundException();
            }
            return productService.FindProductsByCategoryIdAndStatus(categoryId, productStatus);
        }
    }
}