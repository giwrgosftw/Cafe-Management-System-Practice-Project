using Cafe_management_system_backend.MVC.Models;
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

        /// <summary>Adds a new category to the system if it doesn't already exist.</summary>
        /// <param name="category">The Category object to be added.</param>
        /// <exception cref="DuplicateNameException">Thrown if the category name already exists.</exception>
        public void AddCategory(Category category)
        {
            if (category.name == null)
            {
                logger.Error("[CategoryService:AddCategory()] Exception: Category name NOT given.");
                throw new Exception();
            }
            Category categoryDB = categoryRepository.FindByName(category.name);
            if (categoryDB == null)
            {
                // If category name not exist (since new), add new Category
                categoryRepository.AddCategory(category);
                logger.Info("[CategoryService:AddCategory() Success]: Category (Name: {CategoryName}) was created successfully!", category.name);
            }
            else
            {
                // Return an error message
                logger.Error("[CategoryService:AddCategory()] Exception: Category already exists (Category: {CategoryName})", category.name);
                throw new DuplicateNameException("Category already exists. Please use a different category name.");
            }
        }

        /// <summary>Retrieves a list of all categories in the system.</summary>
        /// <returns>A List of Category objects representing all categories.</returns>
        public List<Category> FindAllCategories()
        {
            return categoryRepository.FindAll();
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

        /// <summary>Updates the properties of an existing Category entity based on the provided Category object.</summary>
        /// <param name="category">The Category object containing updated information.</param>
        /// <returns>The updated Category entity.</returns>
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
                throw new KeyNotFoundException();
            }
        }
    }
}