using Microsoft.EntityFrameworkCore;
using CSE443_Project.Models;
using System;
using System.Linq;

namespace CSE443_Project.Data
{
    public static class SeedData
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Make sure the database is created
            context.Database.EnsureCreated();

            // Seed Users if they don't exist
            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    new User
                    {
                        Username = "admin",
                        Password = "admin123", // In production, use password hashing
                        Email = "admin@jobportal.com",
                        Phone = "1234567890",
                        Address = "123 Admin St",
                        City = "Admin City",
                        CreatedAt = DateTime.Now.AddMonths(-6),
                        IsActive = true
                    },
                    new User
                    {
                        Username = "company1",
                        Password = "company123",
                        Email = "hr@techcorp.com",
                        Phone = "2345678901",
                        Address = "456 Tech Blvd",
                        City = "Tech City",
                        CreatedAt = DateTime.Now.AddMonths(-5),
                        IsActive = true
                    },
                    new User
                    {
                        Username = "company2",
                        Password = "company123",
                        Email = "hr@financeco.com",
                        Phone = "3456789012",
                        Address = "789 Finance Ave",
                        City = "Finance City",
                        CreatedAt = DateTime.Now.AddMonths(-4),
                        IsActive = true
                    },
                    new User
                    {
                        Username = "jobseeker1",
                        Password = "seeker123",
                        Email = "john.doe@example.com",
                        Phone = "4567890123",
                        Address = "101 Seeker St",
                        City = "Job City",
                        CreatedAt = DateTime.Now.AddMonths(-3),
                        IsActive = true
                    },
                    new User
                    {
                        Username = "jobseeker2",
                        Password = "seeker123",
                        Email = "jane.smith@example.com",
                        Phone = "5678901234",
                        Address = "202 Applicant Ave",
                        City = "Resume City",
                        CreatedAt = DateTime.Now.AddMonths(-2),
                        IsActive = true
                    }
                );
                context.SaveChanges();
            }

            // Seed Employers
            if (!context.Employers.Any())
            {
                var employer1User = context.Users.FirstOrDefault(u => u.Username == "company1");
                var employer2User = context.Users.FirstOrDefault(u => u.Username == "company2");

                context.Employers.AddRange(
                    new Employer
                    {
                        UserId = employer1User.Id,
                        CompanyName = "Tech Corporation",
                        CompanyDescription = "Leading technology company specializing in software development and IT services.",
                        Industry = "Information Technology",
                        Website = "https://www.techcorp.com",
                        Logo = "/images/logos/techcorp.png",
                        FoundedYear = 2010,
                        CompanySize = 500
                    },
                    new Employer
                    {
                        UserId = employer2User.Id,
                        CompanyName = "Finance Co",
                        CompanyDescription = "Financial services company offering banking, investment, and insurance solutions.",
                        Industry = "Financial Services",
                        Website = "https://www.financeco.com",
                        Logo = "/images/logos/financeco.png",
                        FoundedYear = 2005,
                        CompanySize = 1000
                    }
                );
                context.SaveChanges();
            }

            // Seed JobSeekers
            if (!context.JobSeekers.Any())
            {
                var jobSeeker1User = context.Users.FirstOrDefault(u => u.Username == "jobseeker1");
                var jobSeeker2User = context.Users.FirstOrDefault(u => u.Username == "jobseeker2");

                context.JobSeekers.AddRange(
                    new JobSeeker
                    {
                        UserId = jobSeeker1User.Id,
                        DateOfBirth = new DateTime(1990, 5, 15),
                        Gender = "Male",
                        ProfilePicture = "/images/profiles/john.jpg",
                        Headline = "Software Developer with 5+ years of experience",
                        Summary = "Experienced software developer with a strong background in web development and cloud technologies.",
                        Skills = "C#, ASP.NET Core, SQL, JavaScript, Azure",
                        Education = "Bachelor of Science in Computer Science, University of Technology, 2012",
                        WorkExperience = "Senior Developer at XYZ Tech (2018-Present), Developer at ABC Solutions (2015-2018)"
                    },
                    new JobSeeker
                    {
                        UserId = jobSeeker2User.Id,
                        DateOfBirth = new DateTime(1992, 8, 23),
                        Gender = "Female",
                        ProfilePicture = "/images/profiles/jane.jpg",
                        Headline = "Financial Analyst seeking new opportunities",
                        Summary = "Financial analyst with experience in investment analysis and financial modeling.",
                        Skills = "Financial Analysis, Excel, Financial Modeling, Investment Research, Python",
                        Education = "Master of Business Administration, Finance University, 2016",
                        WorkExperience = "Financial Analyst at Investment Partners (2016-Present)"
                    }
                );
                context.SaveChanges();
            }

            // Seed Job Categories
            if (!context.JobCategories.Any())
            {
                context.JobCategories.AddRange(
                    new JobCategory
                    {
                        Name = "Information Technology",
                        Description = "Jobs related to IT, software development, and computer science",
                        Icon = "fa-laptop-code"
                    },
                    new JobCategory
                    {
                        Name = "Finance",
                        Description = "Jobs related to finance, accounting, and banking",
                        Icon = "fa-money-bill"
                    },
                    new JobCategory
                    {
                        Name = "Marketing",
                        Description = "Jobs related to marketing, advertising, and public relations",
                        Icon = "fa-bullhorn"
                    },
                    new JobCategory
                    {
                        Name = "Engineering",
                        Description = "Jobs related to engineering fields",
                        Icon = "fa-cogs"
                    },
                    new JobCategory
                    {
                        Name = "Healthcare",
                        Description = "Jobs related to healthcare and medical fields",
                        Icon = "fa-stethoscope"
                    }
                );
                context.SaveChanges();
            }

            // Seed Jobs
            if (!context.Jobs.Any())
            {
                var techCorp = context.Employers.FirstOrDefault(e => e.CompanyName == "Tech Corporation");
                var financeCo = context.Employers.FirstOrDefault(e => e.CompanyName == "Finance Co");

                var itCategory = context.JobCategories.FirstOrDefault(c => c.Name == "Information Technology");
                var financeCategory = context.JobCategories.FirstOrDefault(c => c.Name == "Finance");
                var marketingCategory = context.JobCategories.FirstOrDefault(c => c.Name == "Marketing");

                context.Jobs.AddRange(
                    new Job
                    {
                        EmployerId = techCorp.Id,
                        CategoryId = itCategory.Id,
                        JobTitle = ".NET Developer",
                        JobDescription = "We are looking for a skilled .NET developer to join our team and work on challenging projects.",
                        Requirements = "- 3+ years of experience in C# and ASP.NET Core\n- Experience with Entity Framework\n- Knowledge of SQL Server\n- Familiarity with front-end technologies (HTML, CSS, JavaScript)",
                        Benefits = "- Competitive salary\n- Health insurance\n- Flexible working hours\n- Professional development opportunities",
                        JobType = "Full-time",
                        ExperienceLevel = "Mid-Senior",
                        Location = "Tech City",
                        Industry = "Information Technology",
                        SalaryMin = 70000,
                        SalaryMax = 90000,
                        IsRemote = true,
                        IsActive = true,
                        Vacancies = 2,
                        CreatedAt = DateTime.Now.AddDays(-30),
                        Deadline = DateTime.Now.AddDays(30)
                    },
                    new Job
                    {
                        EmployerId = techCorp.Id,
                        CategoryId = marketingCategory.Id,
                        JobTitle = "Digital Marketing Specialist",
                        JobDescription = "Seeking a Digital Marketing Specialist to drive our online marketing initiatives.",
                        Requirements = "- 2+ years of experience in digital marketing\n- Knowledge of SEO/SEM\n- Experience with social media marketing\n- Analytical skills",
                        Benefits = "- Competitive salary\n- Health insurance\n- Flexible working hours\n- Professional development budget",
                        JobType = "Full-time",
                        ExperienceLevel = "Mid",
                        Location = "Tech City",
                        Industry = "Marketing",
                        SalaryMin = 50000,
                        SalaryMax = 65000,
                        IsRemote = false,
                        IsActive = true,
                        Vacancies = 1,
                        CreatedAt = DateTime.Now.AddDays(-20),
                        Deadline = DateTime.Now.AddDays(25)
                    },
                    new Job
                    {
                        EmployerId = financeCo.Id,
                        CategoryId = financeCategory.Id,
                        JobTitle = "Financial Analyst",
                        JobDescription = "Join our financial analysis team to support investment decisions and financial planning.",
                        Requirements = "- Bachelor's degree in Finance or related field\n- 2+ years of experience in financial analysis\n- Strong Excel skills\n- Understanding of financial markets",
                        Benefits = "- Competitive salary\n- Health and dental insurance\n- 401(k) matching\n- Annual bonus",
                        JobType = "Full-time",
                        ExperienceLevel = "Mid",
                        Location = "Finance City",
                        Industry = "Financial Services",
                        SalaryMin = 65000,
                        SalaryMax = 80000,
                        IsRemote = false,
                        IsActive = true,
                        Vacancies = 1,
                        CreatedAt = DateTime.Now.AddDays(-15),
                        Deadline = DateTime.Now.AddDays(45)
                    },
                    new Job
                    {
                        EmployerId = financeCo.Id,
                        CategoryId = itCategory.Id,
                        JobTitle = "IT Support Specialist",
                        JobDescription = "Support our IT infrastructure and provide technical assistance to employees.",
                        Requirements = "- 2+ years of experience in IT support\n- Knowledge of Windows and network administration\n- Familiarity with IT security practices\n- Good communication skills",
                        Benefits = "- Competitive salary\n- Health insurance\n- Flexible working hours\n- Training opportunities",
                        JobType = "Full-time",
                        ExperienceLevel = "Entry-Mid",
                        Location = "Finance City",
                        Industry = "Financial Services",
                        SalaryMin = 45000,
                        SalaryMax = 60000,
                        IsRemote = false,
                        IsActive = true,
                        Vacancies = 1,
                        CreatedAt = DateTime.Now.AddDays(-10),
                        Deadline = DateTime.Now.AddDays(20)
                    }
                );
                context.SaveChanges();
            }

            // Seed CVs
            if (!context.CVs.Any())
            {
                var jobSeeker1 = context.JobSeekers.Include(js => js.User)
                    .FirstOrDefault(js => js.User.Username == "jobseeker1");
                var jobSeeker2 = context.JobSeekers.Include(js => js.User)
                    .FirstOrDefault(js => js.User.Username == "jobseeker2");

                context.CVs.AddRange(
                    new CV
                    {
                        JobSeekerId = jobSeeker1.Id,
                        Title = "Software Developer CV",
                        Description = "My professional CV highlighting software development experience",
                        FilePath = "/uploads/cvs/john_doe_dev.pdf",
                        IsDefault = true,
                        CreatedAt = DateTime.Now.AddMonths(-2),
                        UpdatedAt = DateTime.Now.AddDays(-15)
                    },
                    new CV
                    {
                        JobSeekerId = jobSeeker1.Id,
                        Title = "Project Manager CV",
                        Description = "Alternative CV focusing on project management skills",
                        FilePath = "/uploads/cvs/john_doe_pm.pdf",
                        IsDefault = false,
                        CreatedAt = DateTime.Now.AddMonths(-1),
                        UpdatedAt = null
                    },
                    new CV
                    {
                        JobSeekerId = jobSeeker2.Id,
                        Title = "Financial Analyst CV",
                        Description = "CV highlighting financial analysis skills and experience",
                        FilePath = "/uploads/cvs/jane_smith.pdf",
                        IsDefault = true,
                        CreatedAt = DateTime.Now.AddMonths(-1),
                        UpdatedAt = DateTime.Now.AddDays(-5)
                    }
                );
                context.SaveChanges();
            }

            // Seed Applications
            if (!context.Applications.Any())
            {
                var jobSeeker1 = context.JobSeekers.Include(js => js.User)
                    .FirstOrDefault(js => js.User.Username == "jobseeker1");
                var jobSeeker2 = context.JobSeekers.Include(js => js.User)
                    .FirstOrDefault(js => js.User.Username == "jobseeker2");

                var devJob = context.Jobs.FirstOrDefault(j => j.JobTitle == ".NET Developer");
                var financeJob = context.Jobs.FirstOrDefault(j => j.JobTitle == "Financial Analyst");

                var devCV = context.CVs.FirstOrDefault(cv => cv.Title == "Software Developer CV");
                var financeCV = context.CVs.FirstOrDefault(cv => cv.Title == "Financial Analyst CV");

                context.Applications.AddRange(
                    new Application
                    {
                        JobId = devJob.Id,
                        JobSeekerId = jobSeeker1.Id,
                        CVId = devCV.Id,
                        ApplicationDate = DateTime.Now.AddDays(-20),
                        CoverLetter = "I am excited to apply for the .NET Developer position at Tech Corporation. With over 5 years of experience in C# and ASP.NET Core development, I believe I would be a valuable addition to your team.",
                        Status = "Shortlisted",
                        Notes = "Candidate has strong technical skills",
                        ReviewDate = DateTime.Now.AddDays(-15)
                    },
                    new Application
                    {
                        JobId = financeJob.Id,
                        JobSeekerId = jobSeeker2.Id,
                        CVId = financeCV.Id,
                        ApplicationDate = DateTime.Now.AddDays(-10),
                        CoverLetter = "I am applying for the Financial Analyst position at Finance Co. My background in financial analysis and investment research makes me a strong candidate for this role.",
                        Status = "Reviewed",
                        Notes = "Promising candidate, schedule interview",
                        ReviewDate = DateTime.Now.AddDays(-5)
                    }
                );
                context.SaveChanges();
            }

            // Seed SavedJobs
            if (!context.SavedJobs.Any())
            {
                var jobSeeker1 = context.JobSeekers.Include(js => js.User)
                    .FirstOrDefault(js => js.User.Username == "jobseeker1");
                var jobSeeker2 = context.JobSeekers.Include(js => js.User)
                    .FirstOrDefault(js => js.User.Username == "jobseeker2");

                var itSupportJob = context.Jobs.FirstOrDefault(j => j.JobTitle == "IT Support Specialist");
                var marketingJob = context.Jobs.FirstOrDefault(j => j.JobTitle == "Digital Marketing Specialist");

                context.SavedJobs.AddRange(
                    new SaveJob
                    {
                        JobSeekerId = jobSeeker1.Id,
                        JobId = itSupportJob.Id,
                        SavedAt = DateTime.Now.AddDays(-5),
                        Notes = "Interesting position, consider applying later"
                    },
                    new SaveJob
                    {
                        JobSeekerId = jobSeeker2.Id,
                        JobId = marketingJob.Id,
                        SavedAt = DateTime.Now.AddDays(-3),
                        Notes = "Different from my field but worth exploring"
                    }
                );
                context.SaveChanges();
            }

            // Seed Candidates
            if (!context.Candidates.Any())
            {
                var application = context.Applications.FirstOrDefault(a => a.Status == "Shortlisted");

                if (application != null)
                {
                    context.Candidates.Add(
                        new Candidate
                        {
                            ApplicationId = application.Id,
                            JobId = application.JobId,
                            JobSeekerId = application.JobSeekerId,
                            Status = "Shortlisted",
                            InterviewNotes = null,
                            InterviewDate = DateTime.Now.AddDays(5),
                            ShortlistedDate = DateTime.Now.AddDays(-10)
                        }
                    );
                    context.SaveChanges();
                }
            }
        }
    }
}