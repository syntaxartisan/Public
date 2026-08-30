using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

var users = new[]
{
    new
    {
        Id = "1",
        Name = "Bob",
        Role = "Administrator"
    },
    new
    {
        Id = "2",
        Name = "Susan",
        Role = "Users"
    }
};

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

string issuer = "OperationsKnowledge";
string audience = "OperationsKnowledge.Api";
// key is read from secrets.json file
string key = configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT signing key is not configured.");

Console.WriteLine($"API key length: {key.Length}");
Console.WriteLine($"API key hash: {Convert.ToHexString(
    System.Security.Cryptography.SHA256.HashData(
        Encoding.UTF8.GetBytes(key)))}");

foreach (var user in users)
{
    Console.WriteLine();
    Console.WriteLine("Generating token for {0} ({1})", user.Name, user.Role);

    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        new Claim(JwtRegisteredClaimNames.Name, user.Name),
        new Claim(ClaimTypes.Role, user.Role),
    };

    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    {
        KeyId = "OperationsKnowledgeDev"
    };
    var credentials = new SigningCredentials(
        securityKey,
        SecurityAlgorithms.HmacSha256);
    var token = new JwtSecurityToken(
        issuer: issuer,
        audience: audience,
        claims: claims,
        expires: DateTime.UtcNow.AddHours(1),
        signingCredentials: credentials);
    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
    Console.WriteLine(tokenString);
}
