using Microsoft.EntityFrameworkCore;
using CSE443_Project.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace CSE443_Project.Data;

public partial class ApplicationDbContext : DbContext
{
    private readonly string _connectionString;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, string connectionString = "DefaultConnection")
        : base(options)
    {
        _connectionString = connectionString;
    }

    // DbSet properties for all models
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Employer> Employers { get; set; } = null!;
    public DbSet<JobSeeker> JobSeekers { get; set; } = null!;
    public DbSet<CV> CVs { get; set; } = null!;
    public DbSet<JobCategory> JobCategories { get; set; } = null!;
    public DbSet<Job> Jobs { get; set; } = null!;
    public DbSet<Application> Applications { get; set; } = null!;
    public DbSet<SaveJob> SavedJobs { get; set; } = null!;
    public DbSet<Candidate> Candidates { get; set; } = null!;

    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Setting> Settings { get; set; }





    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString(_connectionString);
            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure relationships between entities

        // User relationships
        modelBuilder.Entity<User>()
            .HasOne(u => u.JobSeeker)
            .WithOne(js => js.User)
            .HasForeignKey<JobSeeker>(js => js.UserId);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Employer)
            .WithOne(e => e.User)
            .HasForeignKey<Employer>(e => e.UserId);

        // Job relationships
        modelBuilder.Entity<Job>()
            .HasOne(j => j.Employer)
            .WithMany(e => e.Jobs)
            .HasForeignKey(j => j.EmployerId);

        modelBuilder.Entity<Job>()
            .HasOne(j => j.Category)
            .WithMany(c => c.Jobs)
            .HasForeignKey(j => j.CategoryId);

        // CV relationships
        modelBuilder.Entity<CV>()
            .HasOne(cv => cv.JobSeeker)
            .WithMany(js => js.CVs)
            .HasForeignKey(cv => cv.JobSeekerId);

        // Application relationships
        modelBuilder.Entity<Application>()
            .HasOne(a => a.Job)
            .WithMany(j => j.Applications)
            .HasForeignKey(a => a.JobId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Application>()
            .HasOne(a => a.JobSeeker)
            .WithMany(js => js.Applications)
            .HasForeignKey(a => a.JobSeekerId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Application>()
            .HasOne(a => a.CV)
            .WithMany()
            .HasForeignKey(a => a.CVId)
            .OnDelete(DeleteBehavior.NoAction);

        // SaveJob relationships
        modelBuilder.Entity<SaveJob>()
            .HasOne(sj => sj.Job)
            .WithMany(j => j.SavedBy)
            .HasForeignKey(sj => sj.JobId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<SaveJob>()
            .HasOne(sj => sj.JobSeeker)
            .WithMany(js => js.SavedJobs)
            .HasForeignKey(sj => sj.JobSeekerId)
            .OnDelete(DeleteBehavior.NoAction);

        // Candidate relationships
        modelBuilder.Entity<Candidate>()
            .HasOne(c => c.Application)
            .WithMany()
            .HasForeignKey(c => c.ApplicationId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Candidate>()
            .HasOne(c => c.Job)
            .WithMany(j => j.Candidates)
            .HasForeignKey(c => c.JobId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Candidate>()
            .HasOne(c => c.JobSeeker)
            .WithMany(js => js.Candidacies)
            .HasForeignKey(c => c.JobSeekerId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}