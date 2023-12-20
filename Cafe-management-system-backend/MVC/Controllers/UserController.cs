using Cafe_management_system_backend.MVC.Models;
using Cafe_management_system_backend.MVC.Services.UserServices;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Cafe_management_system_backend.Controllers
{
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        private readonly UserService userService;

        public UserController(UserService userService)
        {
            this.userService = userService;
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
            catch (ApplicationException ex)
            {
                // Catch the ApplicationException (from SignUp) exception and return a BadRequest response
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = ex.Message });
            }
            catch (Exception ex)
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
    }
}
