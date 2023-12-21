using Cafe_management_system_backend.MVC.Models;
using Cafe_management_system_backend.MVC.Services.UserServices;
using Cafe_management_system_backend.MVC.Security;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Linq;
using System.Collections.Generic;
using System.Data;

namespace Cafe_management_system_backend.Controllers
{
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        private readonly UserService userService;
        private readonly UserAuthorityService userAuthorityService;

        public UserController(UserService userService, UserAuthorityService userAuthorityService)
        {
            this.userService = userService;
            this.userAuthorityService = userAuthorityService;
        }

        [HttpPost, Route("signup")]
        public HttpResponseMessage Signup([FromBody] User user) // FromBody = expecting data which are the according Model values
        {
            try
            {
                userService.SignUp(user);
                // Successfully registered
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Successfully Registered!" });
            }
            catch (DuplicateNameException ex)
            {
                // Catch the DuplicateNameException (from SignUp) exception and return a BadRequest response
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = ex.Message });
            }
            catch (Exception)
            {
                // Handle other exceptions as needed
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "Internal Server Error" });
            }
        }

        [HttpPost, Route("login")]
        public HttpResponseMessage Login([FromBody] User user)
        {
            try
            {   
                // Generate the user's token
                var token = userService.Login(user);
                // Successfully logged-in
                return Request.CreateResponse(HttpStatusCode.OK, token);
            }
            catch (UnauthorizedAccessException ex)
            {
                // Failed to login
                return Request.CreateResponse(HttpStatusCode.Unauthorized, new { message = ex.Message });
            }
        }

        [HttpGet, Route("getAllUsers")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage GetAllUsers()
        {
            try
            {
                // Retrieve the authorization token from the request headers
                var token = Request.Headers.GetValues("authorization").First();
                // Check if the user has the "Admin" authority based on the token
                if (!userAuthorityService.HasAuthorityAdmin(token))
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }
                // Retrieve a list of all Users with role = "User"
                List<User> users = userService.FindAllUsers();
                return Request.CreateResponse(HttpStatusCode.OK, users);
            }
            catch (Exception)
            {
                // Handle any unexpected exceptions and return InternalServerError response
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "Internal Server Error" });
            }
        }
    }
}
