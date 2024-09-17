using System.ComponentModel.DataAnnotations;

namespace WealthSnap.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }         // Primary Key
        public string Name { get; set; }            // Category name
        public int? ParentCategoryId { get; set; }   // Nullable Foreign Key for parent category
        public int? UserId { get; set; }             // Nullable Foreign Key for user

        // Navigation properties
        public User User { get; set; }               // Reference to User
        public Category ParentCategory { get; set; } // Reference to parent category
        public ICollection<Category> Subcategories { get; set; } // Child categories
    }
}
