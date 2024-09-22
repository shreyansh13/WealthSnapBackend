namespace WealthSnap.Models
{
    public class User
    {
        public int UserId { get; set; }          // Primary Key
        public string UserName { get; set; }     // User's name
        public string Email { get; set; }        // User's email
        public string PasswordHash { get; set; } // Hashed password for security
        public DateTime CreatedDate { get; set; } // Account creation date
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        // Navigation property for related expenses
        public virtual ICollection<Expense> Expenses { get; set; }
        // Navigation property for categories
        public virtual ICollection<Category> Categories { get; set; }
    }

}
