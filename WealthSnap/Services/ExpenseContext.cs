using Microsoft.EntityFrameworkCore;
using WealthSnap.Models;

namespace WealthSnap.Services
{
    public class ExpenseContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public DbSet<User> Users { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Category> Categories { get; set; }

        public ExpenseContext(DbContextOptions<ExpenseContext> options, IConfiguration configuration)
    : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Check if optionsBuilder has already been configured
            if (!optionsBuilder.IsConfigured)
            {
                // Retrieve the connection string from configuration
                var connectionString = _configuration.GetConnectionString("FinanceDatabase");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Expense>()
                .Property(e => e.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<User>()
               .HasIndex(u => u.UserName)
               .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasMany(u => u.Categories)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Or another behavior depending on your needs
        }
    }

}
