using Cafe_management_system_backend.MVC.Models;
using Cafe_management_system_backend.MVC.Repositories;
using NLog;
using System.Data;
using System;
using System.Collections.Generic;

namespace Cafe_management_system_backend.MVC.Services
{
    public class ProductServiceImpl : ProductService
    {
        private static Logger logger = LogManager.GetLogger("NLogger");
        private readonly ProductRepository productRepository;

        public ProductServiceImpl(ProductRepository productRepository)
        {
                this.productRepository = productRepository;
        }

        /// <summary> Adds a new product to the system if it doesn't already exist. </summary>
        /// <param name="product"> The Product object to be added. </param>
        /// <exception cref="DuplicateNameException"> Thrown if the product with the same name already exists. </exception>
        public void AddProduct(Product product)
        {
            if (product.name == null)
            {
                logger.Error("[ProductService:Add()] Exception: Product name NOT given.");
                throw new Exception();
            }
            Product productDB = productRepository.FindByName(product.name);
            if (productDB == null)
            {
                // If product not exist (since new), add the new Product
                product.status = "true"; // enable it by default
                productRepository.Add(product);
                logger.Info("[ProductService:Add() Success]: Product (Name: {ProductName}) was created successfully!", product.name);
            }
            else
            {
                // Return an error message
                logger.Error("[ProductService:Add()] Exception: Product already exists (Product: {ProductName})", product.name);
                throw new DuplicateNameException("Product already exists. Please use a different product name.");
            }
        }

        /// <summary> Updates a product in the system and returns the updated Product object. </summary>
        /// <param name="product"> The Product object with updated information. </param>
        /// <returns> The updated Product object. </returns>
        public Product UpdateProduct(Product product)
        {
            Product productDB = UpdateProductEntity(product); // update hard-coded
            productRepository.Update(productDB);  // update/save into DB
            logger.Info("[ProductService:UpdateProduct()] Success: Product updated successfully (Id: {ProductId} & Name: {ProductName})", productDB.id, productDB.name);
            return productDB;
        }

        /// <summary> Finds a product in the system by its ID. </summary>
        /// <param name="productId"> The ID of the product to be found. </param>
        /// <returns> The Product object if found, otherwise null. </returns>
        public Product FindProductById(int productId)
        {
            return productRepository.FindById(productId);
        }

        /// <summary> Retrieves a list of all products in the system. </summary>
        /// <returns> A list of Product objects. </returns>
        public List<Product> FindAllProducts()
        {
            return productRepository.FindAll();
        }

        /// <summary> Retrieves a list of products in the system based on category ID and status. </summary>
        /// <param name="categoryId"> The ID of the category. </param>
        /// <param name="productStatus"> The status of the products to be retrieved. </param>
        /// <returns> A list of Product objects matching the specified category ID and status. </returns>
        public List<Product> FindProductsByCategoryIdAndStatus(int categoryId, string productStatus)
        {
            return productRepository.FindAllByCategoryIdAndStatus(categoryId, productStatus);
        }

        /// <summary> Deletes a product from the system. </summary>
        /// <param name="product"> The Product object to be deleted. </param>
        /// <exception cref="DbUpdateException"> Thrown error when the specified product ID is not found. </exception>
        public void DeleteProduct(int productId)
        {
            Product productDB = FindProductById(productId);
            if (productDB == null)
            {
                logger.Error("[ProductService:DeleteProduct()] Failed: Product with given Id NOT found (Id: {ProductId})", productId);
                throw new KeyNotFoundException();
            }
            productRepository.Delete(productDB);
        }

        /// <summary> Updates a Product entity before these changes be saved in the DB. </summary>
        /// <param name="product"> The Product object with updated information. </param>
        /// <returns> The updated Product entity. </returns>
        /// <exception cref="KeyNotFoundException"> Thrown when the specified product ID is not found. </exception>
        private Product UpdateProductEntity(Product product)
        {
            Product productDB = FindProductById(product.id);
            if (productDB != null)
            {
                productDB.name = product.name ?? productDB.name; // if productDB.name = NULL, do not change it
                productDB.categoryId = product.categoryId ?? productDB.categoryId;
                productDB.description = product.description ?? productDB.description;
                productDB.price = product.price ?? productDB.price;
                return productDB;
            }
            else
            {
                logger.Error("[ProductService:UpdateProductEntity()] Failed: Product with given Id NOT found (Id: {ProductId})", product.id);
                throw new KeyNotFoundException();
            }
        }

    }
}