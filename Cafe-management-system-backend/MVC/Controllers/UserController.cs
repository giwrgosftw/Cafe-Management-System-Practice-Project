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
        private ResponseMessage responseMessage;

        public UserController(CommonUserService commonUserService, UserService userService, 
            UserAuthorityService userAuthorityService, UserEmailService userEmailService)
        {
            this.commonUserService = commonUserService;
            this.userService = userService;
            this.userAuthorityService = userAuthorityService;
            this.userEmailService = userEmailService;
            this.responseMessage = new ResponseMessage();
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
                responseMessage.message = "Successfully Registered!";
                return Request.CreateResponse(HttpStatusCode.OK, responseMessage );
            }
            catch(ArgumentException ex)
            {
                responseMessage.message = ex.Message;
                return Request.CreateResponse(HttpStatusCode.BadRequest, responseMessage);
            }
            catch (DuplicateNameException ex)
            {
                // Catch the DuplicateNameException (from SignUp) exception and return a BadRequest response
                responseMessage.message = ex.Message;
                return Request.CreateResponse(HttpStatusCode.Conflict, responseMessage);
            }
            catch (Exception)
            {
                // Handle other exceptions as needed
                responseMessage.message = "Internal Server Error";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, responseMessage);
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
                responseMessage.message = ex.Message;
                return Request.CreateResponse(HttpStatusCode.Unauthorized, responseMessage);
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
                responseMessage.message = "Internal Server Error";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, responseMessage);
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
                responseMessage.message = "User Updated Successfully!";
                return Request.CreateResponse(HttpStatusCode.OK, responseMessage);
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
                responseMessage.message = "Password Updated Successfully!";
                return Request.CreateResponse(HttpStatusCode.OK, responseMessage);
            }
            catch (ArgumentException ex)
            {
                responseMessage.message = ex.Message;
                return Request.CreateResponse(HttpStatusCode.BadRequest, responseMessage);
            }
            catch (InvalidOperationException)
            {
                responseMessage.message = "Incorrect Old Password";
                return Request.CreateResponse(HttpStatusCode.Conflict);

            }
            catch (Exception)
            {
                responseMessage.message = "Internal Server Error";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, responseMessage);
            }
        }

        /// <summary>API-Call that sends a password reset email to the user.</summary>
        /// <param name="user">The User object containing the user's email.</param>
        [HttpPost, Route("forgotPassword")]
        [CustomAuthenticationFilter]
        public async Task<HttpResponseMessage> ForgotPassword([FromBody] User user)
        {
            responseMessage.message = "A Password was sent to your email successfully";

            try
            {
                await userEmailService.SendForgotPasswordEmail(user);
                return Request.CreateResponse(HttpStatusCode.OK, responseMessage);
            }
            catch (KeyNotFoundException)
            {
                // Because we do not want let the hackers know that the process has failed,
                // will give a fake message on the front, but through our Service's Logger
                // we will distinguish the actual error
                return Request.CreateResponse(HttpStatusCode.OK, responseMessage);
            }
            catch (SmtpException)
            {
                // Handle SmtpException separately and return InternalServerError
                responseMessage.message = "Internal Server Error";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, responseMessage);
            }
            catch (Exception)
            {
                responseMessage.message = "Internal Server Error";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, responseMessage);
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
                responseMessage.message = "User Deleted Successfully!";
                return Request.CreateResponse(HttpStatusCode.OK, responseMessage);
            }
            catch (KeyNotFoundException ex)
            {
                responseMessage.message = ex.Message;
                return Request.CreateResponse(HttpStatusCode.NotFound, responseMessage);
            }
            catch (UnauthorizedAccessException)
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }
            catch (Exception)
            {
                responseMessage.message = "Internal Server Error";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, responseMessage);
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
                responseMessage.message = "Your Account Deleted Successfully!";
                return Request.CreateResponse(HttpStatusCode.OK, responseMessage);
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
