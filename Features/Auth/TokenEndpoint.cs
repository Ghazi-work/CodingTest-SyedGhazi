// DEV/TEST ONLY — hardcoded credentials for Swagger testing.
// Username: admin | Password: password123
// This endpoint must be removed or replaced before any production deployment.

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ProductsApi.Features.Auth;

public record TokenRequest(string Username, string Password);

public static class TokenEndpoint
{
    public static void MapTokenEndpoint(this WebApplication app)
    {
        app.MapPost("/auth/token", (TokenRequest request, IConfiguration config) =>
        {
            if (request.Username != "admin" || request.Password != "password123")
                return Results.Json(new { error = "Invalid credentials" }, statusCode: 401);

            var jwtSettings = config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: [new Claim(ClaimTypes.Name, request.Username)],
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: creds);

            return Results.Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }).AllowAnonymous();
    }
}
