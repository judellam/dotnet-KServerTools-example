using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace server.Components;

internal class JwtGenerator {
    public static string Generate(int userId, string username, string email) {
        // Key for signing the token
        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(Constants.Jwt.Secret));
        SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

        // Claims (add more if needed)
        Claim[] claims = [
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, username),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];

        // Create the token
        JwtSecurityToken token = new(
            issuer: Constants.Jwt.Issuer,
            audience: Constants.Jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}