using CSE443_Project.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace CSE443_Project.Data
{
    public static class SeedData
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Check if the database already has data
            if (context.Users.Any())
            {
                return; // Database has been seeded
            }

            // Add admin user
            var adminUser = new User
            {
                Username = "admin",
                Email = "admin@example.com",
                Password = "admin123", // In a real app, this should be hashed
                Phone = "123-456-7890",
                Address = "Admin Street",
                City = "Admin City",
                CreatedAt = DateTime.Now,
                IsActive = true
            };
            context.Users.Add(adminUser);
            context.SaveChanges();

            // Add employer user
            var employerUser = new User
            {
                Username = "employer",
                Email = "employer@example.com",
                Password = "employer123", // In a real app, this should be hashed
                Phone = "123-456-7891",
                Address = "Employer Street",
                City = "Employer City",
                CreatedAt = DateTime.Now,
                IsActive = true
            };
            context.Users.Add(employerUser);

            // Add job seeker user
            var jobSeekerUser = new User
            {
                Username = "jobseeker",
                Email = "jobseeker@example.com",
                Password = "jobseeker123", // In a real app, this should be hashed
                Phone = "123-456-7892",
                Address = "Seeker Street",
                City = "Seeker City",
                CreatedAt = DateTime.Now,
                IsActive = true
            };
            context.Users.Add(jobSeekerUser);
            context.SaveChanges();

            // Add employer profile
            var employer = new Employer
            {
                UserId = employerUser.Id,
                CompanyName = "Example Company",
                Description = "This is an example company for testing.",
                Website = "https://example.com",
                LogoUrl = "/uploads/logos/default.png",
                Industry = "Technology",
                CompanySize = "50-100",
                Address = "Example City",
                Email = "contact@example.com",
                Phone = "123-456-7899",
                ContactPerson = "John Doe",
                RegisteredDate = DateTime.Now,
                IsVerified = true,
                IsActive = true,
                JobPostingsCount = 1
            };
            context.Employers.Add(employer);

            // Add job seeker profile
            var jobSeeker = new JobSeeker
            {
                UserId = jobSeekerUser.Id,
                DateOfBirth = new DateTime(1990, 1, 1),
                Gender = "Male",
                ProfilePicture = "/uploads/profiles/default.png",
                Headline = "Experienced Software Developer",
                Summary = "Software developer with 5+ years of experience in web development.",
                Skills = "C#, ASP.NET Core, SQL, JavaScript, HTML, CSS",
                Education = "Bachelor's in Computer Science",
                WorkExperience = "5+ years in software development"
            };
            context.JobSeekers.Add(jobSeeker);
            context.SaveChanges();

            // Add job category
            var category = new JobCategory
            {
                Name = "Software Development",
                Description = "Jobs related to software development and programming."
            };
            context.JobCategories.Add(category);
            context.SaveChanges();

            // Add job
            var job = new Job
            {
                EmployerId = employer.Id,
                CategoryId = category.Id,
                JobTitle = "ASP.NET Core Developer",
                JobDescription = "We are looking for an experienced ASP.NET Core developer to join our team.",
                Requirements = "3+ years of experience with ASP.NET Core\nStrong C# skills\nFamiliarity with Entity Framework Core\nExperience with SQL Server",
                Benefits = "Competitive salary\nFlexible work hours\nRemote work options\nHealth insurance",
                Location = "Example City",
                SalaryMin = 80000,
                SalaryMax = 100000,
                JobType = "Full-time",
                ExperienceLevel = "Mid-senior",
                Industry = "Technology",
                IsRemote = false,
                Vacancies = 2,
                IsActive = true,
                CreatedAt = DateTime.Now,
                Deadline = DateTime.Now.AddMonths(1)
            };
            context.Jobs.Add(job);
            context.SaveChanges();

            // Add CV
            var cv = new CV
            {
                JobSeekerId = jobSeeker.Id,
                Title = "Software Developer CV",
                Description = "My professional resume for software development positions",
                FilePath = "/uploads/cvs/sample-cv.pdf",
                IsDefault = true,
                CreatedAt = DateTime.Now
            };
            context.CVs.Add(cv);
            
            // Add another CV for the same job seeker
            var cv2 = new CV
            {
                JobSeekerId = jobSeeker.Id,
                Title = "Web Developer CV",
                Description = "My specialized CV for web development roles",
                FilePath = "/uploads/cvs/web-developer-cv.pdf",
                IsDefault = false,
                CreatedAt = DateTime.Now.AddDays(-10)
            };
            context.CVs.Add(cv2);
            context.SaveChanges();

            // Add application
            var application = new Application
            {
                JobId = job.Id,
                JobSeekerId = jobSeeker.Id,
                CVId = cv.Id,
                CoverLetter = "I am excited to apply for this position...",
                Status = "Pending",
                ApplicationDate = DateTime.Now,
                ReviewDate = null
            };
            context.Applications.Add(application);
            context.SaveChanges();

            // Add candidate (shortlisted job seeker)
            var candidate = new Candidate
            {
                JobId = job.Id,
                JobSeekerId = jobSeeker.Id,
                ApplicationId = application.Id,
                Status = "Shortlisted",
                InterviewNotes = "Good candidate with relevant experience.",
                ShortlistedDate = DateTime.Now,
                InterviewDate = null
            };
            context.Candidates.Add(candidate);
            context.SaveChanges();

            // Add saved job
            var savedJob = new SaveJob
            {
                JobId = job.Id,
                JobSeekerId = jobSeeker.Id,
                SavedAt = DateTime.Now,
                Notes = "Interesting position to apply for later."
            };
            context.SavedJobs.Add(savedJob);
            context.SaveChanges();
        }
    }
} 