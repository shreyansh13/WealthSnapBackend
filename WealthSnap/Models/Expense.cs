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
        public int CategoryId { get; set; }  // Foreign Key to Category
        public string Notes { get; set; }
        public bool Recurring { get; set; }
    }
}
