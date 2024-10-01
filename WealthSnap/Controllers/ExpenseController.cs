using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WealthSnap.Models;
using WealthSnap.Services;

namespace WealthSnap.Controllers
{
    [ApiController]
    [Route("api/Expense")]
    [Authorize]
    public class ExpenseController : Controller
    {
        private readonly ExpenseContext _context;

        public ExpenseController(ExpenseContext context)
        {
            _context = context;
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetExpenses()
        {
            var expenses = await _context.Expenses
    .Include(e => e.Category) // Assuming you have a navigation property for Category
    .ToListAsync();

            var result = expenses.Select(e => new
            {
                e.TransactionId, 
                e.Date,
                e.Amount,
                e.Currency,
                CategoryId = e.Category.CategoryId, // Accessing the Category ID
                CategoryName = e.Category.Name, // Accessing the Category Name
                e.Notes,
                e.Recurring
            });
            return Ok(result);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddExpense([FromBody] Expense expense)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            expense.UserId = TokenHandler.GetUserIdFromToken(token);
            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetExpenses), new { id = 1 }, expense);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateExpense(int id, [FromBody] Expense updatedExpense)
        {
            // Validate the ID
            if (id <= 0)
            {
                return BadRequest("Invalid expense ID.");
            }

            // Validate the input data
            if (updatedExpense == null)
            {
                return BadRequest("Expense data is null.");
            }

            if (id != updatedExpense.TransactionId)
            {
                return BadRequest("Expense ID in the URL does not match the ID in the request body.");
            }

            // Retrieve the existing expense from the database
            var existingExpense = await _context.Expenses.FindAsync(id);

            if (existingExpense == null)
            {
                return NotFound($"Expense with ID {id} not found.");
            }

            // Update the existing expense with new values
            existingExpense.Date = updatedExpense.Date;
            existingExpense.Amount = updatedExpense.Amount;
            existingExpense.Currency = updatedExpense.Currency;
            existingExpense.CategoryId = updatedExpense.CategoryId;
            existingExpense.Notes = updatedExpense.Notes;
            existingExpense.Recurring = updatedExpense.Recurring;

            // Save changes to the database
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "An error occurred while updating the expense.");
            }

            // Return the updated expense
            return Ok(existingExpense);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            // Validate the ID
            if (id <= 0)
            {
                return BadRequest("Invalid expense ID.");
            }

            // Retrieve the existing expense from the database
            var expense = await _context.Expenses.FindAsync(id);

            if (expense == null)
            {
                return NotFound($"Expense with ID {id} not found.");
            }

            // Remove the expense
            _context.Expenses.Remove(expense);

            // Save changes to the database
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "An error occurred while deleting the expense.");
            }

            // Return a status indicating successful deletion
            return NoContent(); // HTTP 204 No Content
        }
    }
}
