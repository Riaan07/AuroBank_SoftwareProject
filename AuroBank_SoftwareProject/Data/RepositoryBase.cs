using System.Linq.Expressions;
using System;
using AuroBank_SoftwareProject.Data.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace AuroBank_SoftwareProject.Data
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        private readonly BankDbContext _context;

        public RepositoryBase(BankDbContext context)
        {
            _context = context;
        }

        public async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<string> GetUsernameByIdAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user.UserName;
        }

        public async Task<T> GetByIdAsync(int? id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task RemoveAsync(int? id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(T? entity)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
