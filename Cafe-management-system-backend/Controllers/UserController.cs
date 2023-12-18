using Cafe_management_system_backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Web.Http;

namespace Cafe_management_system_backend.Controllers
{
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        CafeEntities db = new CafeEntities();

        /// <summary>
        /// A method for SignUp
        /// FromBody = expecting data which are the according Model values
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost, Route("signup")]
        public HttpResponseMessage Signup([FromBody] User user)
        {
            try
            {

                // TODO: THE FOLLOWING PART OF CODE COULD BE IN A SERVICE CLASS

                // Check if the user exists
                User userObjDB = db.Users
                    .Where(u => u.email == user.email).FirstOrDefault();
                if (userObjDB == null)
                {
                    // If e-mail not exist (since new), setup the appropriate User values
                    user.role = "user"; // by default is a user
                    user.status = "false"; // never logged-in before
                    // Add to the database
                    db.Users.Add(user);
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Successfully Registered!" });
                }
                else
                {
                    // Return error if the 
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = "Email Already exists" });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost, Route("login")]
        public HttpResponseMessage Login([FromBody] User user)
        {
            try
            {
                User userObj = db.Users
                    .Where(u => (u.email == user.email && u.password == user.password)).FirstOrDefault();
                if (userObj != null)
                {
                    if (userObj.status == "true")
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new { token = TokenManager.GenerateToken(userObj.email, userObj.password) });
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.Unauthorized, new { message = "Wait for Admin Approval" });
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { message = "Incorrect Username or Passwrod" });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}
