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
using System.Threading.Tasks;
using System.Net.Mail;

namespace Cafe_management_system_backend.Controllers
{
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        private readonly CommonUserService commonUserService;
        private readonly UserService userService;
        private readonly UserAuthorityService userAuthorityService;
        private readonly UserEmailService userEmailService;

        public UserController(CommonUserService commonUserService, UserService userService, 
            UserAuthorityService userAuthorityService, UserEmailService userEmailService)
        {
            this.commonUserService = commonUserService;
            this.userService = userService;
            this.userAuthorityService = userAuthorityService;
            this.userEmailService = userEmailService;
        }

        /// <summary>API-Call for registering a new user.</summary>
        /// <param name="user">The User object containing user information.</param>
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

        /// <summary>API-Call that logs-in a user and generates an authentication token.</summary>
        /// <param name="user">The User object containing login credentials.</param>
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

        /// <summary>API-Call that gets a list of all users (with Role='User').</summary>
        [HttpGet, Route("getAllUsers")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage GetAllUsers()
        {
            try
            {
                // Retrieve the authorization token from the request headers
                var token = Request.Headers.GetValues("authorization").First();
                // Check if the user has the "Admin" authority based on the token
                if (!userAuthorityService.HasAuthorityAdmin(token)) { return Request.CreateResponse(HttpStatusCode.Unauthorized); }
                // Retrieve a list of all Users with role = "User"
                List<User> users = commonUserService.FindAllUsers();
                return Request.CreateResponse(HttpStatusCode.OK, users);
            }
            catch (Exception)
            {
                // Handle any unexpected exceptions and return InternalServerError response
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "Internal Server Error" });
            }
        }

        /// <summary>API-Call that updates user information.</summary>
        /// <param name="user">The User object containing updated information.</param>
        [HttpPost, Route("update")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage Update([FromBody] User user)
        {
            try
            {
                // Retrieve the authorization token from the request headers
                var token = Request.Headers.GetValues("authorization").First();
                // Check if the user has the "Admin" authority based on the token
                if (!userAuthorityService.HasAuthorityAdmin(token)) { return Request.CreateResponse(HttpStatusCode.Unauthorized); }
                // Update
                userService.UpdateUser(user);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "User Updated Successfully!" });
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "Internal Server Error" });
            }
        }

        /// <summary>API-Call that changes the user's password.</summary>
        /// <param name="changePassword">The ChangePassword object containing old and new password.</param>
        [HttpPost, Route("changePassword")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage ChangeUserPassword(ChangePassword changePassword)
        {
            try
            {
                // Retrieve the authorization token from the request headers
                var token = Request.Headers.GetValues("authorization").First();
                // Get Principal info
                PrincipalProfile principalProfile = TokenManager.GetPrincipalProfileInfo(token);
                // Change password
                userService.ChangeUserPassword(principalProfile, changePassword);
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Password Updated Successfully!" });
            }
            catch (InvalidOperationException)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = "Incorrect Old Password" });

            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "Internal Server Error" });
            }
        }

        /// <summary>API-Call that sends a password reset email to the user.</summary>
        /// <param name="user">The User object containing the user's email.</param>
        [HttpPost, Route("forgotPassword")]
        [CustomAuthenticationFilter]
        public async Task<HttpResponseMessage> ForgotPassword([FromBody] User user)
        {
            string messageResponse = "A Password was sent to your email successfully";

            try
            {
                await userEmailService.SendForgotPasswordEmail(user);
                return Request.CreateResponse(HttpStatusCode.OK, new { messageResponse });
            }
            catch (KeyNotFoundException)
            {
                // Because we do not want let the hackers know that the process has failed,
                // will give a fake message on the front, but through our Service's Logger
                // we will distinguish the actual error
                return Request.CreateResponse(HttpStatusCode.OK, new { messageResponse });
            }
            catch (SmtpException)
            {
                // Handle SmtpException separately and return InternalServerError
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "Internal Server Error" });
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "Internal Server Error" });
            }
        }

        /// <summary> Deletes a user from the system based on user authorization and returns an HTTP response. </summary>
        /// <param name="userId"> The ID of the user to be deleted. </param>
        /// <returns> HttpResponseMessage indicating success or appropriate error messages. </returns>
        [HttpPost, Route("deleteUser")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage DeleteUser([FromUri] int userId)
        {
            try
            {
                // Retrieve the authorization token from the request headers
                var token = Request.Headers.GetValues("authorization").First();
                // Check if the user has the "Admin" authority based on the token
                if (!userAuthorityService.HasAuthorityAdmin(token)) { return Request.CreateResponse(HttpStatusCode.Unauthorized); }
                // Since authorized, delete the User
                userService.DeleteUser(userId);
                // Return Success Response
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "User Deleted Successfully!" });
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "Internal Server Error" });
            }
        }

        /// <summary> Deletes the logged-in/Principal account, ensuring authorization, and returns an HTTP response. </summary>
        /// <returns> HttpResponseMessage indicating success or appropriate error messages. </returns>
        [HttpPost, Route("deleteMyAccount")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage DeleteMyAccount()
        {
            try
            {
                // Retrieve the authorization token from the request headers
                var token = Request.Headers.GetValues("authorization").First();
                // Check if the user has the "Admin" authority based on the token
                if (!userAuthorityService.HasAuthorityAdmin(token)) { return Request.CreateResponse(HttpStatusCode.Unauthorized); }
                // Get Principal info
                PrincipalProfile principalProfile = TokenManager.GetPrincipalProfileInfo(token);
                // Since authorized, delete the User
                userService.DeleteMyAccount(principalProfile.Email);
                // Return Success Response
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Your Account Deleted Successfully!" });
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "Internal Server Error" });
            }
        }
    }
}
