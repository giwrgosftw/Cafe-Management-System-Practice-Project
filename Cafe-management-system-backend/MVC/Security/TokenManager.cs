using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Cafe_management_system_backend.MVC.Security
{
    public class TokenManager
    {
        // 1. Define a hard-coded random JWT secret key to secure the APIs
        //    TODO: Hard-coding a secret key in the source code is not a recommended practice for production systems
        public static string secretKey = "DAFHdS3PnA3mdHYJ-IslVN3pMQ3jHQY-200Sh1ThGwc";

        /// <summary> 2. This method Generates a JWT token based on the provided email and role. </summary>
        /// <param name="email">The email address of the user.</param>
        /// <param name="role">The role of the user.</param>
        /// <returns>The generated JWT token as a string.</returns>
        public static string GenerateToken(string email, string role)
        {
            // 3. Create a SymmetricSecurityKey using the secret/security key
            //    SymmetricSecurityKey objects requires a UTF-8 encoded byte array as its input  
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            // 4. Define a SecurityTokenDescriptor to specify token details
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                // 5. Set the subject of the token with claims/user info (e.g., email and role)
                //    ClaimsIdentity accepts as input an generic type array of Claim objects
                //    (the type can be inferred from the elements)
                Subject = new ClaimsIdentity(
                    new[] { 
                    new Claim(ClaimTypes.Email, email), 
                    new Claim(ClaimTypes.Role, role) 
                    }
                ),
                // 6. Set the expiration time for the token (8 hours in this case)
                Expires = DateTime.UtcNow.AddHours(8),
                // 7. Specify the signing credentials using the security key and algorithm
                //    The key will be used to validate that the JWT token still belongs to that logged-in user
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
            };
            // 8. Create a JwtSecurityTokenHandler (so that to create the token after)
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            // 9. Create a JWT token based on the descriptor
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
            // 10. Write the token as a string and return it
            return handler.WriteToken(token);
        }

        /// <summary> This method validates the provided (Principal's) token and returns the corresponding ClaimsPrincipal. </summary>
        /// <param name="token">The JWT token to be validated.</param>
        /// <returns>The ClaimsPrincipal if validation is successful, otherwise null.</returns>
        public static ClaimsPrincipal ValidateTokenAndGetPrincipal(string token)
        {
            try
            {   // Initialize a JwtSecurityTokenHandler to handle JWT-related operations (e.g., reading and validating tokens)
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                // Parse the input token (string) and convert it into a JwtSecurityToken object,
                // which is a representation of the decoded JWT
                JwtSecurityToken jwtToken = (JwtSecurityToken)tokenHandler.ReadJwtToken(token);

                if (jwtToken == null)
                {
                    return null;
                }

                // Before validating the token we need to define some criteria (that will be checked during the validation)
                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true, // JWT must contain an expiration time
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)) // (sign the JWT) the server uses this key to validate the signature of the incoming JWT
                };
                // Use the token handler to validate the token based on the above criteria
                SecurityToken securityToken;
                // If validation is successful, get Principal (logged-in user identity and roles)
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, parameters, out securityToken);
                // securityToken = JWT token value after the validation (passed by reference)
                return principal;

            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary> Gets the Principal's profile data (email and role) from the particular (validated) token. </summary>
        /// <param name="rawToken">The raw token containing the token type and actual token value.</param>
        /// <returns>The PrincipalProfile object containing email and role information.</returns>
        public static PrincipalProfile GetPrincipalProfileInfo(string RawToken)
        {
            // E.g., in Bearer token format, the RawToken is a string that includes
            // both a token type and the actual token value, separated by a space (' ')
            string[] array = RawToken.Split(' '); // E.g., "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
            var token = array[1];   // We need to second part (token only)
            // Get the Principal user aligns to that token
            ClaimsPrincipal principal = ValidateTokenAndGetPrincipal(token);
            if (principal == null)
            {
                return null;
            }
            // Get Principal Identity data
            ClaimsIdentity identity = null;
            try
            {
                identity = (ClaimsIdentity)principal.Identity;
            }
            catch (Exception)
            {
                return null;
            }
            // Return the required identity data in the form of our PrincipalProfile Object
            PrincipalProfile principalProfile = new PrincipalProfile();
            principalProfile.Email = identity.FindFirst(ClaimTypes.Email).Value;
            principalProfile.Role = identity.FindFirst(ClaimTypes.Role).Value;
            return principalProfile;

        }

    }
}