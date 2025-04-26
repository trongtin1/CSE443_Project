using Microsoft.EntityFrameworkCore;
using CSE443_Project.Models;
using System.Collections.Generic;

namespace CSE443_Project.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Setting> Settings { get; set; }
    }
}
