using Microsoft.EntityFrameworkCore;
namespace Admin.Data;

public partial class ApplicationDbContext : DbContext
{

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // public virtual DbSet<User> Users { get; set; }




}