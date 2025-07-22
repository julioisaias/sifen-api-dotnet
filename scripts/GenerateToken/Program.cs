using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

// Configuración JWT (misma que en appsettings.Test.json)
var key = "TuClaveSecretaSuperSeguraDeAlMenos256BitsParaProduccion";
var issuer = "SifenApi";
var audience = "SifenApiUsers";
var durationInMinutes = 60;

// Crear claims
var claims = new[]
{
    new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
    new Claim(ClaimTypes.Name, "testuser@test.com"),
    new Claim(ClaimTypes.Email, "testuser@test.com"),
    new Claim("ruc", "80000000-1"),
    new Claim("role", "Admin")
};

// Generar token
var tokenHandler = new JwtSecurityTokenHandler();
var keyBytes = Encoding.UTF8.GetBytes(key);
var tokenDescriptor = new SecurityTokenDescriptor
{
    Subject = new ClaimsIdentity(claims),
    Expires = DateTime.UtcNow.AddMinutes(durationInMinutes),
    SigningCredentials = new SigningCredentials(
        new SymmetricSecurityKey(keyBytes),
        SecurityAlgorithms.HmacSha256Signature),
    Issuer = issuer,
    Audience = audience
};

var token = tokenHandler.CreateToken(tokenDescriptor);
var tokenString = tokenHandler.WriteToken(token);

Console.WriteLine("=== TOKEN JWT GENERADO ===");
Console.WriteLine(tokenString);
Console.WriteLine();
Console.WriteLine("=== PARA USAR EN PRUEBAS ===");
Console.WriteLine($"Authorization: Bearer {tokenString}");
Console.WriteLine();
Console.WriteLine("=== API KEY PARA PRUEBAS ===");
Console.WriteLine("x-api-key: test-api-key-123");
Console.WriteLine();
Console.WriteLine("=== VÁLIDO HASTA ===");
Console.WriteLine(DateTime.UtcNow.AddMinutes(durationInMinutes).ToString("yyyy-MM-dd HH:mm:ss UTC"));