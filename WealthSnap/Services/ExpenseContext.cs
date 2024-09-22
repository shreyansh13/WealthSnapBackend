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

            // Configure foreign key relationships
            modelBuilder.Entity<Expense>()
                .HasOne<User>(e => e.User)  // Assuming you have a User entity defined
                .WithMany(u => u.Expenses)
                .HasForeignKey(e => e.UserId)                
                .OnDelete(DeleteBehavior.NoAction); // Set delete behavior if needed

            modelBuilder.Entity<Expense>()
                .HasOne<Category>(e => e.Category)
                .WithMany()
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.NoAction); // Set delete behavior if needed

            modelBuilder.Entity<Category>()
                .HasOne<User>(c => c.User)
                .WithMany(u => u.Categories)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<User>()
                    .HasIndex(u => u.UserName)
                    .IsUnique();

            modelBuilder.Entity<User>()
                    .HasIndex(u => u.Email)
                    .IsUnique();
        }
    }

}
