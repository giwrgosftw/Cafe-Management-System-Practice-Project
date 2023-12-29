﻿using Cafe_management_system_backend.MVC.Models;
using Cafe_management_system_backend.MVC.Security;
using Cafe_management_system_backend.MVC.Services;
using Cafe_management_system_backend.MVC.Services.UserServices;
using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Cafe_management_system_backend.MVC.Controllers
{
    [RoutePrefix("api/category")]
    public class CategoryController : ApiController
    {
        private readonly CategoryService categoryService;
        private readonly UserAuthorityService userAuthorityService;

        public CategoryController(CategoryService categoryService, UserAuthorityService userAuthorityService)
        {
            this.categoryService = categoryService;
            this.userAuthorityService = userAuthorityService;
        }

        /// <summary>
        /// Adds a new category to the system via HTTP POST request, 
        /// accessible through the "addNewCategory" route.
        /// </summary>
        /// <param name="category">The Category object to be added.</param>
        /// <returns>
        ///   Returns an HTTP response indicating success or failure.
        ///   - If successful, returns 200 OK with a success message.
        ///   - If the category name already exists, returns 400 Bad Request with an error message.
        ///   - If an unexpected error occurs, returns 500 Internal Server Error with an error message.
        /// </returns>
        [HttpPost, Route("addNewCategory")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage AddNewCategory([FromBody] Category category)
        {
            try
            {
                // Retrieve the authorization token from the request headers
                var token = Request.Headers.GetValues("authorization").First();
                // Check if the user has the "Admin" authority based on the token
                if (!userAuthorityService.HasAuthorityAdmin(token)) { return Request.CreateResponse(HttpStatusCode.Unauthorized); }
                // Since authorized, add the new Category
                categoryService.AddCategory(category);
                // Return Successful Response
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Category Successfully Added!" });
            }
            catch (DuplicateNameException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = ex.Message });
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "Internal Server Error" });
            }
        }

        /// <summary>
        /// Retrieves a list of all categories in the system via HTTP GET request, 
        /// accessible through the "getAllCategories" route.
        /// </summary>
        /// <returns>
        ///   Returns an HTTP response with a List of Category objects representing all categories if successful.
        ///   If an unexpected error occurs, returns 500 Internal Server Error with an error message.
        /// </returns>
        [HttpGet, Route("getAllCategories")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage GetAllCategories()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, categoryService.FindAllCategories());
            }
            catch(Exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "Internal Server Error" });
            }
        }

        /// <summary>Updates an existing category in the system via HTTP POST request, accessible through the "updateCategory" route.</summary>
        /// <param name="category">The Category object containing updated information.</param>
        /// <returns>
        ///   Returns an HTTP response indicating success or failure.
        ///   - If successful, returns 200 OK with a success message.
        ///   - If the user lacks "Admin" authority, returns 401 Unauthorized.
        ///   - If an unexpected error occurs, returns 500 Internal Server Error with an error message.
        /// </returns>
        [HttpPost, Route("updateCategory")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage UpdateCategory([FromBody] Category category)
        {
            try
            {
                // Retrieve the authorization token from the request headers
                var token = Request.Headers.GetValues("authorization").First();
                // Check if the user has the "Admin" authority based on the token
                if (!userAuthorityService.HasAuthorityAdmin(token)) { return Request.CreateResponse(HttpStatusCode.Unauthorized); }
                // Since authorized, update the new Category
                categoryService.UpdateCategory(category);
                // Return Success Response
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Category Updated Successfully!" });
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "Internal Server Error" });
            }
        }
    }
}