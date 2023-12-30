using Cafe_management_system_backend.MVC.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace Cafe_management_system_backend.MVC.Repositories
{
    public class CategoryRepositoryImpl : ConnectionRepositoryDB, CategoryRepository
    {

        /// <summary>Finds a category by its name in the system.</summary>
        /// <param name="categoryName">The name of the category to be found.</param>
        /// <returns>The Category object if found, otherwise null.</returns>
        public Category FindByName(string categoryName)
        {
            try
            {
                return db.Categories.Where(c => c.name == categoryName).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error($"[CategoryRepository:FindByName()] Exception: {ex.Message}");
                GetInnerException(ex);
                throw;
            }
        }

        /// <summary>Retrieves a list of all categories in the system.</summary>
        /// <returns>A List of Category objects representing all categories.</returns>
        public List<Category> FindAll()
        {
            try
            {
               return db.Categories.ToList();
            }
            catch (Exception ex)
            {
                logger.Error($"[CategoryRepository:FindAll()] Exception: {ex.Message}");
                GetInnerException(ex);
                throw;
            }
        }

        /// <summary>Finds a category by its unique id in the system.</summary>
        /// <param name="categoryId">The unique id of the category to be found.</param>
        /// <returns>The Category object if found, otherwise null.</returns>
        public Category FindById(int? categoryId)
        {
            try
            {
                return db.Categories.Find(categoryId);
            }
            catch (Exception ex)
            {
                logger.Error($"[CategoryRepository:FindById()] Exception: {ex.Message}");
                GetInnerException(ex);
                throw;
            }
        }

        /// <summary> Adds a new category to the system. </summary>
        /// <param name="category">The Category object containing information about the new category.</param>
        public void Add(Category category)
        {
            try
            {
                db.Categories.Add(category);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error($"[CategoryRepository:Add()] Exception: {ex.Message}");
                GetInnerException(ex);
                throw;
            }
        }

        /// <summary>Updates an existing category in the system.</summary>
        /// <param name="category">The Category object containing updated information.</param>
        public void Update(Category category)
        {
            try
            {
                // User Entity is considered as Modified form
                db.Entry(category).State = System.Data.Entity.EntityState.Modified;
                // Save Updated user
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                logger.Error($"[CategoryRepository:Update()] Exception: {ex.Message}");
                GetInnerException(ex);
                throw;
            }
        }

        /// <summary> Deletes a category from the system. </summary>
        /// <param name="category"> The Category object to be deleted. </param>
        /// <exception cref="Exception"> Thrown when an error occurs during the delete operation. </exception>
        public void Delete(Category category)
        {
            try
            {
                db.Categories.Remove(category);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.Error($"[CategoryRepository:Delete()] Exception: {ex.Message}");
                GetInnerException(ex);
                throw;
            }
        }
    }
}