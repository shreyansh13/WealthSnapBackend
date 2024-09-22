using System.ComponentModel.DataAnnotations;

namespace WealthSnap.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }         // Primary Key
        public string Name { get; set; }            // Category name
        public int? ParentCategoryId { get; set; }   // Nullable Foreign Key for parent category

        public int UserId { get; set; }
        public virtual User? User { get; set; }

    }
}
