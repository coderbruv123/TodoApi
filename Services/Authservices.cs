using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using Todoapp.Context;
using Todoapp.Entities;

namespace Todoapp.Services{
    public class AuthServices(ApplicationContext context, IConfiguration configuration) : IAuthServices
    {
    public async Task<string?> LoginAsync(UserDTO request)
    {
    var user = await context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
    if (user == null)
    {
        return null;
    }

    var passwordVerificationResult = new PasswordHasher<User>().VerifyHashedPassword(user, user.Password, request.Password);
    if (passwordVerificationResult == PasswordVerificationResult.Failed)
    {
        return null;
    }
    return CreateToken(user);

    }

        public async Task<User?> RegisterAsync(UserDTO request)
        {
        var user =  await context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
        if (user != null)
        {
            return null;

            }
        user = new User
        {
                Username = request.Username,
                Password = request.Password
            };
    var hashedPassword = new PasswordHasher<User>().HashPassword(user, request.Password);

    user.Username = request.Username;
    user.Password = hashedPassword;

    context.Users.Add(user);
    context.SaveChanges();

    return user;        }
    
    
    private string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };
        var key = new SymmetricSecurityKey (System.Text.Encoding.UTF8.GetBytes(configuration.GetValue<string>("Jwt:Key")));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var tokenDescriptor=  new JwtSecurityToken(
            issuer: configuration.GetValue<string>("Jwt:Issuer"),
            audience: configuration.GetValue<string>("Jwt:Audience"),
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    
    }
}}