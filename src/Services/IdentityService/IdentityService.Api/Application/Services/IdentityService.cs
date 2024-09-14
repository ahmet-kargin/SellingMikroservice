using IdentityService.Api.Application.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IdentityService.Api.Application.Services;

public class IdentityService : IIdentityService
{
    private readonly IConfiguration _configuration;

    // Dependency injection ile IConfiguration alınır
    public IdentityService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<LoginResponseModel> Login(LoginRequestModel requestModel)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, requestModel.UserName),
            new Claim(ClaimTypes.Name, "Ahmet"),
        };

        // appsettings.json'dan JwtSettings değerlerini alıyoruz
        var secretKey = _configuration["JwtSettings:SecretKey"];
        var issuer = _configuration["JwtSettings:Issuer"];
        var audience = _configuration["JwtSettings:Audience"];

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.Now.AddDays(10);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.Now,
            expires: expiry,
            signingCredentials: creds
        );

        // Token'ı oluşturuyoruz
        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(token);

        LoginResponseModel response = new()
        {
            UserToken = encodedJwt,
            UserName = requestModel.UserName
        };

        return Task.FromResult(response);
    }
}
