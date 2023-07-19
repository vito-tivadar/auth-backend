using System.Text;
using System.Collections.ObjectModel;
using System.Security.Claims;
using auth_backend.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace auth_backend.Utility;

// Helper class for Authentication (using JWT)
public static class AuthenticationHelpers
{
    public static TokenValidationParameters GetTokenValidationParameters(string JwtSecret)
    {
        return new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSecret)),
            ValidateIssuer = false,
            ValidateAudience = false,
        };
    }

    public static string CreateJWT(UserModel user, string JWT_secret, int expirationInHours = 12)
    {
        // DateTime expirationDate = DateTime.Now.AddHours(expirationInHours);

        Collection<Claim> claims = user.GetClaims();

        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWT_secret));
        System.Console.WriteLine(JWT_secret);
        SigningCredentials credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256Signature
        );

        JwtSecurityToken token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddHours(expirationInHours),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
};
