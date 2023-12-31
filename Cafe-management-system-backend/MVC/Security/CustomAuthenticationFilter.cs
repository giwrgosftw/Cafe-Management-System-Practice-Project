﻿using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Http.Results;

namespace Cafe_management_system_backend.MVC.Security
{
    /// <summary>
    /// The Class Implements the IHttpActionResult interface, indicating that
    /// instances of this class can be used to create HTTP responses.
    /// Represents an HTTP response indicating authentication failure (invalid token).
    /// </summary>
    public class AuthenticationFailureResult : IHttpActionResult
    {
        public AuthenticationFailureResult() { }

        /// <summary>
        /// The following method is part of 'IHttpActionResult' and it has to be implemented.
        /// The method returns an HTTP response indicating authentication failure (Unauthorized - 401/Invalid token).
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An asynchronous task representing the HTTP response.</returns>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            // Specifically with a status code of Unauthorized (401)
            HttpResponseMessage responseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            // Because the method is asynchronous, the response has to be return in a form of a Task
            return Task.FromResult(responseMessage);
        }
    }

    /// <summary>
    /// Custom authentication filter implementing IAuthenticationFilter.
    /// Responsible for determining client authentication and handling authentication challenges.
    /// </summary>
    public class CustomAuthenticationFilter : AuthorizeAttribute, IAuthenticationFilter
    {
        /// <summary>
        /// This method is responsible for determining whether the client
        /// should be authenticated based on the provided HTTP request (overrides IAuthenticationFilter)
        /// </summary>
        /// <param name="context">The HTTP authentication context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An asynchronous task representing the authentication process.</returns>
        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            // Extracting the Authorization header from the HTTP request
            AuthenticationHeaderValue authorization = context.Request.Headers.Authorization;
            // It checks whether the header exists, the scheme is "Bearer," and the parameter is not empty.
            if (authorization == null || authorization.Scheme != "Bearer" || string.IsNullOrEmpty(authorization.Parameter))
            {   // If any of these conditions is not met, it sets the ErrorResult, indicating authentication failure
                context.ErrorResult = new AuthenticationFailureResult();
                return; // immediately exits the method
            }
            // Else, validate the token and obtain the logged-in user (Prinicpal) instance/data
            context.Principal = TokenManager.ValidateTokenAndGetPrincipal(authorization.Parameter);
        }

        /// <summary>
        /// This method is invoked when the authentication challenge is triggered (e.g., prevent access without loggin) - (overrides IAuthenticationFilter).
        /// Responsible for responding to validations, using the Authorization header to modify the HTTP response and inform the browser about grant access
        /// </summary>
        /// <param name="context">The HTTP authentication challenge context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An asynchronous task representing the challenge process.</returns>
        public async Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {   // Authenticate the user and get the http response
            var result = await context.Result.ExecuteAsync(cancellationToken);
            // The browser, upon receiving the user's credentials, includes them in the Authorization header
            if (result.StatusCode == HttpStatusCode.Unauthorized)
            {   // If the response has a status code of Unauthorized (401), prompt the user to retry authentication
                // Note: we use the (often used) 'Www-Authenticate' Authorization header here
                result.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue("Basic", "real=localhost")); // Basic authentication technique, (TODO: change the) localhost is the protected are
            }
            // If successful, grants access to the protected resource
            context.Result = new ResponseMessageResult(result);
        }
    }
}