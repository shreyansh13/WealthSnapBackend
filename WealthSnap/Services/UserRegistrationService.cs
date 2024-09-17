using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WealthSnap.Models;

namespace WealthSnap.Services
{
    public interface IUserRegistrationService
    {
        Task<bool> UserExistsAsync(string email, string username);
        Task<bool> RegisterUserAsync(UserRegistrationRequest user);
    }

    public class UserRegistrationService : IUserRegistrationService
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher; // Use a password hasher service
        private readonly JwtService _jwtService; // Inject JwtService

        public UserRegistrationService(AppDbContext context, IPasswordHasher<User> passwordHasher, JwtService jwtService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        public async Task<bool> UserExistsAsync(string email, string username)
        {
            return await _context.Users.AnyAsync(u => u.Email == email || u.UserName == username);
        }

        public async Task<bool> RegisterUserAsync(UserRegistrationRequest request)
        {
            var user = new User
            {
                UserName = request.UserName
            };

            // Hash the password
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
            user.RefreshToken = _jwtService.GenerateRefreshToken();
            user.Email = request.Email;
            user.CreatedDate = DateTime.UtcNow;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return true;
        }  
    }

}
