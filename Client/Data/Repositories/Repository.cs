using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CSE443_Project.Data.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public Task UpdateAsync(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(T entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }
            _dbSet.Remove(entity);
            return Task.CompletedTask;
        }

        public async Task DeleteByIdAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                await DeleteAsync(entity);
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            return entity != null;
        }

        public async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}