using Cafe_management_system_backend.MVC.Models;
using Cafe_management_system_backend.MVC.Security;
using System.Data;
using System.Net;
using System;
using System.Net.Http;
using System.Web.Http;
using Cafe_management_system_backend.MVC.Services.UserServices;
using System.Linq;
using Cafe_management_system_backend.MVC.Services.Facades;
using Cafe_management_system_backend.MVC.Services;


namespace Cafe_management_system_backend.MVC.Controllers
{
    [RoutePrefix("api/product")]
    public class ProductController : ApiController
    {
        private readonly UserAuthorityService userAuthorityService;
        private readonly ProductCategoryFacadeService productCategoryFacadeService;
        private readonly ProductService productService;

        public ProductController(UserAuthorityService userAuthorityService, 
            ProductCategoryFacadeService productCategoryFacadeService, ProductService productService)
        {
            this.userAuthorityService = userAuthorityService;
            this.productCategoryFacadeService = productCategoryFacadeService;
            this.productService = productService;
        }

        /// <summary> Adds a new Product to the system (via HTTP POST) if the user has "Admin" authority. </summary>
        /// <param name="product"> The Product object to be added. </param>
        /// <returns> HttpResponseMessage indicating success or appropriate error messages. </returns>
        [HttpPost, Route("addNewProduct")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage AddNewProduct([FromBody] Product product)
        {
            try
            {
                // Retrieve the authorization token from the request headers
                var token = Request.Headers.GetValues("authorization").First();
                // Check if the user has the "Admin" authority based on the token
                if (!userAuthorityService.HasAuthorityAdmin(token)) { return Request.CreateResponse(HttpStatusCode.Unauthorized); }
                // If has authority add the new Product
                productCategoryFacadeService.AddProductWithCategory(product);
                // Return Successful Response
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Product Successfully Added!" });
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

        /// <summary> Retrieves a list of all products in the system and returns an HTTP response. </summary>
        /// <returns> HttpResponseMessage containing the list of Product objects or an error message. </returns>
        [HttpGet, Route("getAllProducts")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage GetAllProducts()
        {
            try
            {
                var result = productService.FindAllProducts();
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch(Exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "Internal Server Error" });
            }
        }

        /// <summary> Retrieves a list of products in the system based on category ID 
        /// and Product's status = "true", returning an HTTP response. 
        /// </summary>
        /// <param name="categoryId"> The ID of the category. </param>
        /// <returns> HttpResponseMessage containing the list of Product objects or an error message. </returns>
        //[HttpGet, Route("getProductsByCategory/{id}")]
        //[CustomAuthenticationFilter]
        //public HttpResponseMessage GetProductsByCategory(int id)
        [HttpGet, Route("getProductsByCategory")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage GetProductsByCategory([FromUri] int categoryId)
        {
            try
            {
                var result = productCategoryFacadeService.GetProductsByCategoryIdAndStatus(categoryId, "true");
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "Internal Server Error" });
            }
        }


        /// <summary> Retrieves a product in the system by its ID and returns an HTTP response. </summary>
        /// <param name="productId"> The ID of the product to be retrieved. </param>
        /// <returns> HttpResponseMessage containing the Product object or an error message. </returns>
        [HttpGet, Route("getProductById")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage GetProductById([FromUri] int productId)
        {
            try
            {
                var result = productService.FindProductById(productId);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch(Exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "Internal Server Error" });
            }
        }


        /// <summary> Updates a product in the system based on user authorization and returns an HTTP response. </summary>
        /// <param name="product"> The Product object with updated information. </param>
        /// <returns> HttpResponseMessage indicating success or appropriate error messages. </returns>
        [HttpPost, Route("updateProduct")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage UpdateProduct([FromBody] Product product)
        {
            try
            {
                // Retrieve the authorization token from the request headers
                var token = Request.Headers.GetValues("authorization").First();
                // Check if the user has the "Admin" authority based on the token
                if (!userAuthorityService.HasAuthorityAdmin(token)) { return Request.CreateResponse(HttpStatusCode.Unauthorized); }
                // Since authorized, update the new Product
                productService.UpdateProduct(product);
                // Return Success Response
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Category Updated Successfully!" });
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "Internal Server Error" });
            }
        }

        /// <summary> Deletes a product from the system based on user authorization and returns an HTTP response. </summary>
        /// <param name="productId"> The ID of the product to be deleted. </param>
        /// <returns> HttpResponseMessage indicating success or appropriate error messages. </returns>
        [HttpPost, Route("deleteProduct")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage DeleteProduct([FromUri] int productId)
        {
            try
            {
                // Retrieve the authorization token from the request headers
                var token = Request.Headers.GetValues("authorization").First();
                // Check if the user has the "Admin" authority based on the token
                if (!userAuthorityService.HasAuthorityAdmin(token)) { return Request.CreateResponse(HttpStatusCode.Unauthorized); }
                // Since authorized, delete the Product
                productService.DeleteProduct(productId);
                // Return Success Response
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Product Deleted Successfully!" });
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "Internal Server Error" });
            }
        }

    }
}