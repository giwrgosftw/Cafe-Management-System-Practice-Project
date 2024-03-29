﻿using Cafe_management_system_backend.MVC.Models;
using Cafe_management_system_backend.MVC.Repositories;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;

namespace Cafe_management_system_backend.MVC.Services
{
    public class CategoryServiceImpl : CategoryService
    {
        private static Logger logger = LogManager.GetLogger("NLogger");
        private readonly CategoryRepository categoryRepository;

        public CategoryServiceImpl(CategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        /// <summary>Retrieves a list of all categories in the system.</summary>
        /// <returns>A List of Category objects representing all categories.</returns>
        public List<Category> FindAllCategories()
        {
            return categoryRepository.FindAll();
        }

        /// <summary> Retrives a Category object in the system by its unique ID. </summary>
        /// <param name="categoryId"> The ID of the Category to be found. </param>
        /// <returns> The Category object if found, otherwise null. </returns>
        public Category FindCategoryByIdWithoutException(int? categoryId)
        {
            return categoryRepository.FindById(categoryId);
        }

        /// <summary>Finds a category by its ID with exception handling.</summary>
        /// <param name="categoryId">The ID of the category to find.</param>
        /// <returns>The found Category object.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the category with the specified ID is not found.</exception>
        public Category FindCategoryByIdWithException(int? categoryId)
        {
            Category categoryDB = categoryRepository.FindById(categoryId);
            if (categoryDB == null)
            {
                logger.Error("[CategoryService:FindCategoryByIdWithException()] Failed: Category with given Id NOT found (Id: {CategoryId})", categoryId);
                throw new KeyNotFoundException($"Category with given Id NOT found (Id: {categoryId})");
            }
            return categoryDB;
        }

        /// <summary>Adds a new category to the system if it doesn't already exist.</summary>
        /// <param name="category">The Category object to be added.</param>
        /// <exception cref="DuplicateNameException">Thrown if the category name already exists.</exception>
        public void AddCategory(Category category)
        {
            if (category.name == null)
            {
                logger.Error("[CategoryService:Add()] Exception: Category name NOT given.");
                throw new ArgumentException("Category name NOT given.");
            }
            Category categoryDB = categoryRepository.FindByName(category.name);
            if (categoryDB == null)
            {
                // If category name not exist (since new), add new Category
                categoryRepository.Add(category);
                logger.Info("[CategoryService:Add() Success]: Category (Name: {CategoryName}) was created successfully!", category.name);
            }
            else
            {
                // Return an error message
                logger.Error("[CategoryService:Add()] Exception: Category already exists (Category: {CategoryName})", category.name);
                throw new DuplicateNameException("Category already exists. Please use a different category name.");
            }
        }

        /// <summary>Updates an existing category in the system and returns the updated Category object.</summary>
        /// <param name="category">The Category object containing updated information.</param>
        /// <returns>The updated Category object.</returns>
        public Category UpdateCategory(Category category)
        {
            Category categoryDB = UpdateCategoryEntity(category); // update hard-coded
            categoryRepository.Update(categoryDB);  // update/save into DB
            logger.Info("[CategoryService:UpdateCategory()] Success: Category updated successfully (Id: {CategoryId} & Name: {CategoryName})", categoryDB.id, categoryDB.name);
            return categoryDB;
        }

        /// <summary> Deletes a category from the system by its ID, ensuring it is not associated with any products. </summary>
        /// <param name="categoryId"> The ID of the category to be deleted. </param>
        /// <exception cref="KeyNotFoundException"> Thrown when the specified category ID is not found. </exception>
        /// <exception cref="Exception"> Thrown when the category is connected to at least one product. </exception>
        public void DeleteCategory(int categoryId)
        {
            Category categoryDB = FindCategoryByIdWithException(categoryId);
            if (categoryDB.Products.Count > 0)
            {
                logger.Error("[CategoryService:DeleteCategory()] Failed: Cannot delete a Category which is connected with at least one Product (ProductCount: {ProductCount})", categoryDB.Products.Count);
                throw new InvalidOperationException($"Cannot delete a Category which is connected with at least one Product (ProductCount: {categoryDB.Products.Count})");
            }
            categoryRepository.Delete(categoryDB);
        }

        /// <summary>Counts the total number of categories.</summary>
        /// <returns>The total number of categories in the database.</returns>
        public int CountAllCategories()
        {
            return categoryRepository.CountAll();
        }

        /// <summary>Updates the properties of an existing Category entity based on the provided Category object.</summary>
        /// <param name="category">The Category object containing updated information.</param>
        /// <returns>The updated Category entity.</returns>
        /* TODO: this method was implemented for learning purposes, we do not need this extra method, instead,
         * in the Repository class, update using 'db.Entry(objectEntity).CurrentValues.SetValues(updatedObjectEntity);'
        */
        private Category UpdateCategoryEntity(Category category)
        {
            Category categoryDB = categoryRepository.FindById(category.id);
            if (categoryDB != null)
            {
                categoryDB.name = category.name ?? categoryDB.name; // if categoryDB.name = NULL, do not change it
                return categoryDB;
            }
            else
            {
                logger.Error("[CategoryService:UpdateCategoryEntity()] Failed: Category with given Id NOT found (Id: {CategoryId})", category.id);
                throw new KeyNotFoundException($"Category with given Id NOT found (Id: {category.id})");
            }
        }

    }
}