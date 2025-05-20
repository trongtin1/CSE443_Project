using Microsoft.EntityFrameworkCore;
using Admin.Models;

namespace Admin.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CV> CVs { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<AdminUser> AdminUsers { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure CV relationships
            modelBuilder.Entity<CV>()
                .HasOne(c => c.User)
                .WithMany(u => u.CVs)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.user_id);
                entity.Property(e => e.name).IsRequired();
                entity.Property(e => e.email).IsRequired();
                entity.Property(e => e.password).IsRequired();
                entity.Property(e => e.role).IsRequired();
                entity.Property(e => e.created_at).HasDefaultValueSql("GETDATE()");
            });

            // Configure AdminUser entity
            modelBuilder.Entity<AdminUser>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired();
                entity.Property(e => e.Password).IsRequired();
                entity.Property(e => e.Role).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
            });
        }
    }
} 