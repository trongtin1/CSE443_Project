using CSE443_Project.Data;
using CSE443_Project.Models;
using CSE443_Project.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSE443_Project.Services.Implementations
{
    public class JobCategoryService : IJobCategoryService
    {
        private readonly ApplicationDbContext _context;

        public JobCategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<JobCategory> GetJobCategoryByIdAsync(int id)
        {
            return await _context.JobCategories
                .Include(jc => jc.Jobs)
                .FirstOrDefaultAsync(jc => jc.Id == id);
        }

        public async Task<IEnumerable<JobCategory>> GetAllJobCategoriesAsync()
        {
            return await _context.JobCategories
                .OrderBy(jc => jc.Name)
                .ToListAsync();
        }

        public async Task<JobCategory> CreateJobCategoryAsync(JobCategory jobCategory)
        {
            _context.JobCategories.Add(jobCategory);
            await _context.SaveChangesAsync();
            return jobCategory;
        }

        public async Task<JobCategory> UpdateJobCategoryAsync(JobCategory jobCategory)
        {
            _context.Entry(jobCategory).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return jobCategory;
        }

        public async Task<bool> DeleteJobCategoryAsync(int id)
        {
            var jobCategory = await _context.JobCategories.FindAsync(id);
            if (jobCategory == null)
                return false;

            _context.JobCategories.Remove(jobCategory);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetJobCountByCategoryIdAsync(int categoryId)
        {
            return await _context.Jobs.CountAsync(j => j.CategoryId == categoryId);
        }
    }
}