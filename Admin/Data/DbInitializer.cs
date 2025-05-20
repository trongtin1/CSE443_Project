using Admin.Models;

namespace Admin.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // Check if there are any admin users
            if (context.AdminUsers.Any())
            {
                return;   // DB has been seeded
            }

            // Add admin user
            var adminUser = new AdminUser
            {
                Username = "admin",
                Password = "admin123", // In production, this should be hashed
                FullName = "Administrator",
                Role = "Admin",
                CreatedAt = DateTime.UtcNow
            };

            context.AdminUsers.Add(adminUser);
            context.SaveChanges();
        }
    }
} 