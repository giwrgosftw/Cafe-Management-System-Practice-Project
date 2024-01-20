using Cafe_management_system_backend.MVC.Models;
using Cafe_management_system_backend.MVC.Security;
using Cafe_management_system_backend.MVC.Services;
using Cafe_management_system_backend.MVC.Services.UserServices;
using System;
using System.Collections.Generic;
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
        private ResponseMessage responseMessage;

        public CategoryController(CategoryService categoryService, UserAuthorityService userAuthorityService)
        {
            this.categoryService = categoryService;
            this.userAuthorityService = userAuthorityService;
            responseMessage = new ResponseMessage();
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
                responseMessage.message = "Category Successfully Added!";
                return Request.CreateResponse(HttpStatusCode.OK, responseMessage.message);
            }
            catch (ArgumentException ex)
            {
                responseMessage.message = ex.Message;
                return Request.CreateResponse(HttpStatusCode.BadRequest, responseMessage);
            }
            catch (DuplicateNameException ex)
            {
                responseMessage.message = ex.Message;
                return Request.CreateResponse(HttpStatusCode.Conflict, responseMessage);
            }
            catch (Exception)
            {
                responseMessage.message = "Internal Server Error";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, responseMessage);
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
                responseMessage.message = "Internal Server Error";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, responseMessage);
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
                responseMessage.message = "Category Updated Successfully!";
                return Request.CreateResponse(HttpStatusCode.OK, responseMessage);
            }
            catch (KeyNotFoundException ex)
            {
                responseMessage.message = ex.Message;
                return Request.CreateResponse(HttpStatusCode.NotFound, responseMessage);
            }
            catch(Exception)
            {
                responseMessage.message = "Internal Server Error";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, responseMessage);
            }
        }

        /// <summary> Deletes a category from the system based on user authorization and returns an HTTP response. </summary>
        /// <param name="categoryId"> The ID of the category to be deleted. </param>
        /// <returns> HttpResponseMessage indicating success or appropriate error messages. </returns>
        [HttpPost, Route("deleteCategory")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage DeleteCategory([FromUri] int categoryId)
        {
            try
            {
                // Retrieve the authorization token from the request headers
                var token = Request.Headers.GetValues("authorization").First();
                // Check if the user has the "Admin" authority based on the token
                if (!userAuthorityService.HasAuthorityAdmin(token)) { return Request.CreateResponse(HttpStatusCode.Unauthorized); }
                // Since authorized, delete the Category
                categoryService.DeleteCategory(categoryId);
                // Return Success Response
                responseMessage.message = "Category Deleted Successfully!";
                return Request.CreateResponse(HttpStatusCode.OK, responseMessage);
            }
            catch (InvalidOperationException ex)
            {
                responseMessage.message = ex.Message;
                return Request.CreateResponse(HttpStatusCode.Conflict, responseMessage);
            }
            catch (KeyNotFoundException ex)
            {
                responseMessage.message = ex.Message;
                return Request.CreateResponse(HttpStatusCode.NotFound, responseMessage);
            }
            catch (Exception)
            {
                responseMessage.message = "Internal Server Error";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, responseMessage);
            }
        }
    }
}
