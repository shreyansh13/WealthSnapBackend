using Microsoft.EntityFrameworkCore;
using WealthSnap.Models;

namespace WealthSnap.Services
{
    public class ExpenseService
    {
        private readonly ExpenseContext _context;

        public ExpenseService(ExpenseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Expense>> GetExpensesAsync()
        {
            return await _context.Expenses.ToListAsync();
        }

        public async Task AddExpenseAsync(Expense expense)
        {
            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();
        }
    }
}
