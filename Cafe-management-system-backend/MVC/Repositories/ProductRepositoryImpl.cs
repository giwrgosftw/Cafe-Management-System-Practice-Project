using Cafe_management_system_backend.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cafe_management_system_backend.MVC.Repositories
{
    public class ProductRepositoryImpl : ConnectionRepositoryDB, ProductRepository
    {
        /// <summary> Finds a product in the system by its ID. </summary>
        /// <param name="productId"> The ID of the product to be found. </param>
        /// <returns> The Product object if found, otherwise null. </returns>
        public Product FindById(int productId)
        {
            try
            {
                return db.Products.Find(productId);
            }
            catch (Exception ex)
            {
                logger.Error($"[ProductRepository:FindById()] Exception: {ex.Message}");
                GetInnerException(ex);
                throw;
            }
        }

        /// <summary> Finds a product in the database by its name. </summary>
        /// <param name="productName"> The name of the product to be found. </param>
        /// <returns>The Product object if found, otherwise null.</returns>
        public Product FindByName(string productName)
        {
            try
            {
                return db.Products.Where(p => p.name == productName).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error($"[ProductRepository:FindByName()] Exception: {ex.Message}");
                GetInnerException(ex);
                throw;
            }
        }

        /// <summary> Retrieves a list of all products in the system. </summary>
        /// <returns> A list of Product objects. </returns>
        public List<Product> FindAll()
        {
            try
            {
                // Step 1: (Project to an anonymous type) Create a LINQ query that joins the Products and Categories tables
                var resultAnonymous =
                                      // Selecting data from the Product/Category table in the database,
                                      // and you are using the alias 'Product'/'Category' to refer
                                      // to each individual record from the Product/Category table
                                      from Product in db.Products
                                      join Category in db.Categories
                                      // join records where the categoryId in the Products table
                                      // is equal to the id in the Categories table (matching FK with PK).
                                      on Product.categoryId equals Category.id
                                      // Select the columns of each table you want to retrieve
                                      select new
                                      {
                                          // Selecting properties from the 'Product' object
                                          Product.id,
                                          Product.name,
                                          Product.description,
                                          Product.price,
                                          Product.status,
                                          // Selecting properties from the 'Category' object
                                          categoryId = Category.id,
                                          //categoryName = Category.name
                                      };

                // Step 2: Map the anonymous type to the Product type
                var productsMapped = resultAnonymous
                    .AsEnumerable()  // Switch to LINQ to Objects to perform the mapping in memory rather than in the database
                    .Select(p => new Product
                    {
                        id = p.id,
                        name = p.name,
                        categoryId = p.categoryId,
                        description = p.description,
                        price = p.price,
                        status = p.status,
                        // Include Category information in Product object (If bi-directional)
                        //Category = new Category
                        //{
                        //    id = p.categoryId,
                        //    name = p.categoryName
                        //}
                    })
                    .ToList();

                return productsMapped;
            }
            catch (Exception ex)
            {
                logger.Error($"[ProductRepository:FindAll()] Exception: {ex.Message}");
                GetInnerException(ex);
                throw;
            }
        }

        /// <summary> Retrieves a list of products in the system based on category ID and Product's status. </summary>
        /// <param name="categoryId"> The ID of the category. </param>
        /// <param name="productStatus"> The status of the products to be retrieved. </param>
        /// <returns> A list of Product objects matching the specified category ID and Product's status. </returns>
        public List<Product> FindAllByCategoryIdAndStatus(int categoryId, string productStatus)
        {
            try
            {
                // Step 1: Project to an anonymous type
                var resultAnonymous = db.Products
                    .Where(x => x.categoryId == categoryId && x.status == productStatus)
                    .Select(x => new
                    {
                        x.id,
                        x.name,
                        x.categoryId,
                        x.description,
                        x.price,
                        x.status
                    })
                    .ToList();

                // Step 2: Map the anonymous type to the Product type
                var productsMapped = resultAnonymous
                    .AsEnumerable()  // Switch to LINQ to Objects to perform the mapping in memory
                    .Select(p => new Product
                    {
                        id = p.id,
                        name = p.name,
                        categoryId = p.categoryId,
                        description = p.description,
                        price = p.price,
                        status = p.status
                    })
                    .ToList();

                return productsMapped;
            }
            catch (Exception ex)
            {
                logger.Error($"[ProductRepository:FindAllByCategoryIdAndStatus()] Exception: {ex.Message}");
                GetInnerException(ex);
                throw;
            }
        }

        /// <summary> Checks if a product with the specified ID exists in the system. </summary>
        /// <param name="productId"> The ID of the product to be checked. </param>
        /// <returns> True if the product exists, otherwise false. </returns>
        public bool DoesExistById(int productId)
        {
            try
            {
                return db.Products.Any(p => p.id == productId);
            }
            catch (Exception ex)
            {
                logger.Error($"[ProductRepository:DoesExistById()] Exception: {ex.Message}");
                GetInnerException(ex);
                throw;
            }
        }

        /// <summary> Adds a new product to the system. </summary>
        /// <param product="product">The Product object containing information about the new product.</param>
        public void Add(Product product)
        {
            try
            {
                db.Products.Add(product);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error($"[ProductRepository:Add()] Exception: {ex.Message}");
                GetInnerException(ex);
                throw;
            }
        }

        /// <summary> Updates a product in the system. </summary>
        /// <param name="product"> The Product object to be updated. </param>
        /// <exception cref="Exception"> Thrown when an error occurs during the update operation. </exception>
        public void Update(Product product)
        {
            try
            {
                // User Entity is considered as Modified form
                db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                // Save Updated user
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error($"[ProductRepository:Update()] Exception: {ex.Message}");
                GetInnerException(ex);
                throw;
            }
        }

        /// <summary> Deletes a product from the system. </summary>
        /// <param name="product"> The Product object to be deleted. </param>
        /// <exception cref="Exception"> Thrown when an error occurs during the delete operation. </exception>
        public void Delete(Product product)
        {
            try
            {
                db.Products.Remove(product);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error($"[ProductRepository:Delete()] Exception: {ex.Message}");
                GetInnerException(ex);
                throw;
            }
        }

        /// <summary>Counts the total number of products.</summary>
        /// <returns>The total number of products in the database.</returns>
        public int CountAll()
        {
            try
            {
                return db.Products.Count();
            }
            catch (Exception ex)
            {
                logger.Error($"[ProductRepository:CountAll()] Exception: {ex.Message}");
                GetInnerException(ex);
                throw;
            }
        }
    }
}