using Cafe_management_system_backend.MVC.Models;
using Cafe_management_system_backend.MVC.Repositories;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace Cafe_management_system_backend.MVC.Services.UserServices
{
    public class UserEmailServiceImpl : UserEmailService
    {
        private readonly UserRepository userRepository;
        private readonly CommonUserService commonUserService;
        private static Logger logger = LogManager.GetLogger("NLogger");

        public UserEmailServiceImpl(UserRepository userRepository, CommonUserService commonUserService)
        {
            this.userRepository = userRepository;
            this.commonUserService = commonUserService;
        }

        /// <summary>This method sends a 'forgot-password' email to the specified user asynchronously.</summary>
        /// <param name="user">The User object containing user information, including email.</param>
        /// <returns>A Task representing that asynchronous operation.</returns>
        public async Task SendForgotPasswordEmail(User user)
        {
            try
            {
                // Search the user
                User userDB = commonUserService.FindUserByEmail(user.email);
                if (userDB == null)
                {
                    throw new KeyNotFoundException();
                }
                // Create the mail message
                var message = createMailMessage(user, userDB);
                // Use a SmtpClient to send the email message asynchronously
                using (var smtp = new SmtpClient())
                {
                    await smtp.SendMailAsync(message); // Asynchronously send the email message
                    await Task.FromResult(0); // TODO: might NOT needed
                }
            }
            catch(KeyNotFoundException)
            {
                logger.Error($"[UserEmailService:FindUserByEmail()] Failed: User with given Email NOT found (Email: {user.email})");
                throw;
            }
            catch (SmtpException smtpEx)
            {
                logger.Error($"[UserEmailService:SendForgotPasswordEmail()] SmtpException: {smtpEx.Message}");
                throw; // Rethrow the SmtpException to be caught in the controller
            }
            catch (Exception ex)
            {
                logger.Error($"[UserEmailService:SendForgotPasswordEmail()] Exception: Email did NOT send (Email to: {user.email}): {ex.Message}");
                throw new ApplicationException("Failed to send email", ex);
            }
        }

        /// <summary>This method creates the body content for the forgot password email.</summary>
        /// <param name="email">The email address of the user.</param>
        /// <param name="password">The password to be included in the email body.</param>
        /// <returns>The formatted email body content.</returns>
        private string createEmailBody(string email, string password) 
        {
            // Initialize an empty string to store the email body
            string body = string.Empty;
            // Use StreamReader to read the content of an HTML template file
            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("/Templates/Email_Forms/Forgot-password.html")))
            {
                // Read the entire content of the HTML file and store it in the 'body' variable
                body = reader.ReadToEnd();
            }
            // Replace/Fulfill the body's placeholders
            body = body.Replace("{email}", email);
            body = body.Replace("{password}", password);
            body = body.Replace("{frontendUrl}", "http://localhost:4200"); // TODO: need to be changed in Production
            return body;
        }

        /// <summary>This method creates the MailMessage object for the forgot password email.</summary>
        /// <param name="user">The User object containing user information, including email.</param>
        /// <param name="userDB">The User object obtained from the database.</param>
        /// <returns>The configured MailMessage object.</returns>
        private MailMessage createMailMessage(User user, User userDB)
        {
            // Create a new MailMessage instance
            var message = new MailMessage();
            // Finalise the email information
            message.To.Add(new MailAddress(user.email));
            message.Subject = "Password by Cafe Management System";
            message.Body = createEmailBody(user.email, userDB.password);
            message.IsBodyHtml = true; // Specify that the body is in HTML format
            return message;
        }
    }
}