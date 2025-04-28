using Microsoft.EntityFrameworkCore;


namespace Client.Data;

public partial class ApplicationDbContext : DbContext
{

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // public virtual DbSet<User> Users { get; set; }




}