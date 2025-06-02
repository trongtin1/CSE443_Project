using CSE443_Project.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSE443_Project.Services.Interfaces
{
    public interface IJobCategoryService
    {
        Task<JobCategory> GetJobCategoryByIdAsync(int id);
        Task<IEnumerable<JobCategory>> GetAllJobCategoriesAsync();
        Task<JobCategory> CreateJobCategoryAsync(JobCategory jobCategory);
        Task<JobCategory> UpdateJobCategoryAsync(JobCategory jobCategory);
        Task<bool> DeleteJobCategoryAsync(int id);
        Task<int> GetJobCountByCategoryIdAsync(int categoryId);
    }
}