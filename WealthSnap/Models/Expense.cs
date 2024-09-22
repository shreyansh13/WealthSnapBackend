using System.ComponentModel.DataAnnotations;

namespace WealthSnap.Models
{
    public class Expense
    {
        [Key]
        public int TransactionId { get; set; }  // Primary Key
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Notes { get; set; }
        public bool Recurring { get; set; }

        // Foreign key properties
        public int UserId { get; set; }  // Foreign Key for User
        public int CategoryId { get; set; }  // Foreign Key for Category


        public virtual User? User { get; set; }
        public virtual Category? Category { get; set; }
    }
}
