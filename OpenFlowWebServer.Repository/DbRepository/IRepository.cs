using Microsoft.EntityFrameworkCore;
using OpenFlowWebServer.Domain;
using OpenFlowWebServer.Domain.Entities;
using File = OpenFlowWebServer.Domain.Entities.File;

namespace OpenFlowWebServer.Repository.DbRepository
{
    // Generyczny interfejs repozytorium
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T?> GetByIdAsync(object id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task SaveChangesAsync();
    }

    // Generyczna klasa repozytorium wykorzystuj¹ca EF
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<List<T>> GetAllAsync() =>
            await _dbSet.ToListAsync();

        public async Task<T?> GetByIdAsync(object id) =>
            await _dbSet.FindAsync(id);

        public async Task AddAsync(T entity) =>
            await _dbSet.AddAsync(entity);

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();
    }

}
