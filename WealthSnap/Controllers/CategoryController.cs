using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WealthSnap.Models;
using WealthSnap.Services;

namespace WealthSnap.Controllers
{
    [ApiController]
    [Route("api/Category")]
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ExpenseContext _context;

        public CategoryController(ExpenseContext context)
        {
            _context = context;
        }

        [HttpGet("get")]
        public async Task<IActionResult> Get()
        {
            var categories = await _context.Categories.ToListAsync();

            // Create a dictionary to quickly access categories by their Id
            var categoryDtos = categories.Select(c => new CategoryDto
            {
                CategoryId = c.CategoryId,
                Name = c.Name,
                ParentCategoryId = c.ParentCategoryId,
                Subcategories = new List<CategoryDto>() // Initialize the Subcategories list
            }).ToList();

            Dictionary<int, CategoryDto> categoryDictionary = categoryDtos.ToDictionary(c => c.CategoryId);

            // Create the root list for categories without a parent
            var rootCategories = new List<CategoryDto>();

            // Build the hierarchy
            foreach (var categoryDto in categoryDtos)
            {
                if (categoryDto.ParentCategoryId.HasValue)
                {
                    // If it has a parent, add it to the parent's Subcategories list
                    if (categoryDictionary.TryGetValue(categoryDto.ParentCategoryId.Value, out var parentCategory))
                    {
                        parentCategory.Subcategories.Add(categoryDto);
                    }
                }
                else
                {
                    // If no parent, add it to the root categories
                    rootCategories.Add(categoryDto);
                }
            }

            // Return the response model
            var response = new CategoryResponse
            {
                Categories = rootCategories
            };

            return Ok(response);
        }



        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] Category category)
        {
            if (category == null)
            {
                return BadRequest("Category data is null.");
            }

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = category.CategoryId }, category);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid category ID.");
            }

            // Retrieve the category from the database
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound($"Category with ID {id} not found.");
            }

            // Remove the category
            _context.Categories.Remove(category);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Handle cases where the category cannot be deleted due to foreign key constraints
                return StatusCode(500, "An error occurred while deleting the category.");
            }

            // Return no content if deletion is successful
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Category updatedCategory)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid category ID.");
            }

            if (updatedCategory == null || id != updatedCategory.CategoryId)
            {
                return BadRequest("Category data is invalid.");
            }

            // Retrieve the existing category from the database
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound($"Category with ID {id} not found.");
            }

            // Update the existing category with new values
            category.Name = updatedCategory.Name;
            category.ParentCategoryId = updatedCategory.ParentCategoryId;

            // Save changes to the database
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Handle potential errors, such as concurrency issues
                return StatusCode(500, "An error occurred while updating the category.");
            }

            // Return the updated category
            return Ok(category);
        }
    }
}
