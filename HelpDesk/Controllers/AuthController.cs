using HelpDesk.Data;
using HelpDesk.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly HelpdeskContext _context;
    private readonly IConfiguration _config;

    public AuthController(HelpdeskContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest login)
    {
        var user = _context.Users.FirstOrDefault(u => u.Username == login.Username);
        if (user == null)
        {
            Console.WriteLine("User not found");
            return Unauthorized("User not found");
        }

        Console.WriteLine($"[DEBUG] Raw password input: '{login.Password}'");

        var providedHash = HashPassword(login.Password);
        Console.WriteLine($"Comparing hashes:\nExpected: {user.PasswordHash}\nProvided: {providedHash}");

        if (user.PasswordHash != providedHash)
        {
            return Unauthorized("Invalid credentials");
        }

        var token = GenerateJwtToken(user);
        return Ok(new { token });
    }

    [HttpGet("debug-users")]
    public IActionResult DebugUsers()
    {
        var users = _context.Users.ToList();
        return Ok(users);
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("DepartmentId", user.DepartmentId.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("THIS_IS_A_SUPER_SECRET_KEY"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string HashPassword(string password)
    {
        var encoding = new UTF8Encoding(false); // force UTF-8, no BOM
        using var sha = SHA256.Create();
        var bytes = encoding.GetBytes(password); // consistent hash input
        var hash = sha.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
