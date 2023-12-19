using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Cafe_management_system_backend
{
    public class TokenManager
    {
        // 1. Define a hard-coded random JWT secret key to secure the APIs
        //    TODO: Hard-coding a secret key in the source code is not a recommended practice for production systems
        public static string SecurityKeyBytes = "DAFHdS3PnA3mdHYJ-IslVN3pMQ3jHQY-200Sh1ThGwc";

        // 2. Method to generate a JWT token based on provided email and role
        public static string GenerateToken(string email, string role)
        {
            // 3. Create a SymmetricSecurityKey using the secret/security key
            //    SymmetricSecurityKey objects requires a UTF-8 encoded byte array as its input  
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecurityKeyBytes));
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
    }
}