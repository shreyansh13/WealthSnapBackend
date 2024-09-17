namespace WealthSnap.Models
{
    public class User
    {
        public int UserId { get; set; }          // Primary Key
        public string UserName { get; set; }     // User's name
        public string Email { get; set; }        // User's email
        public string PasswordHash { get; set; } // Hashed password for security
        public DateTime CreatedDate { get; set; } // Account creation date

        // Navigation property for categories
        public ICollection<Category> Categories { get; set; }

        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }

}
