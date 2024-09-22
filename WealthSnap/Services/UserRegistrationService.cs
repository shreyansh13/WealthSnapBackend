using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WealthSnap.Models;

namespace WealthSnap.Services
{
    public interface IUserRegistrationService
    {
        Task<bool> UserExistsAsync(string email, string username);
        Task<User> RegisterUserAsync(UserRegistrationRequest user);
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

        public async Task<User> RegisterUserAsync(UserRegistrationRequest request)
        {
            try
            {
                var user = new User
                {
                    UserName = request.UserName
                };

                // Hash the password
                user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
                user.Email = request.Email;
                user.CreatedDate = DateTime.UtcNow;

                user.AccessToken = _jwtService.GenerateAccessToken(user);
                user.RefreshToken = _jwtService.GenerateRefreshToken();
                user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7); // Set refresh token expiration

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return user;
            }
            catch (DbUpdateException ex)
            {
                // Log the exception and handle database-related errors
                // You might want to log this exception using a logging framework
                // e.g., _logger.LogError(ex, "Database update error while registering user.");

                throw new ApplicationException("An error occurred while registering the user. Please try again later.");
            }
            catch (Exception ex)
            {
                // Handle other exceptions (e.g., hashing issues)
                // Log the exception as needed
                // e.g., _logger.LogError(ex, "Unexpected error while registering user.");

                throw new ApplicationException("An unexpected error occurred. Please try again later.");
            }

        }  
    }

}
