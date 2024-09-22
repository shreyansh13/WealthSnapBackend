using Microsoft.EntityFrameworkCore;
using WealthSnap.Models;

namespace WealthSnap.Services
{
    public class CategoryContext : DbContext
    {
        private readonly IConfiguration _configuration;        
        public DbSet<Category> Categories { get; set; }

        public CategoryContext(DbContextOptions<CategoryContext> options, IConfiguration configuration)
    : base(options)
        {
            _configuration = configuration;
        }
    }

}
