using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using WealthSnap.Models;
using PasswordVerificationResult = Microsoft.AspNetCore.Identity.PasswordVerificationResult;

namespace WealthSnap.Services
{
    public interface IUserAuthService
    {
        User Authenticate(string username, string password);
        string HashPassword(User user, string password);
    }

    public class UserAuthService : IUserAuthService
    {
        private readonly AppDbContext _context;

        private readonly IPasswordHasher<User> _passwordHasher;

        public UserAuthService(AppDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public User Authenticate(string username, string password)
        {
            User? user = _context.Users.SingleOrDefault(u => u.UserName == username);

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (result == PasswordVerificationResult.Success)
            {
                return user;
            }
            else
            {
                throw new Exception("Password mismatch");
            }
        }

        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public User GetUserById(int userId)
        {
            return _context.Users.SingleOrDefault(u => u.UserId == userId);
        }

        public string HashPassword(User user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }
    }

}
