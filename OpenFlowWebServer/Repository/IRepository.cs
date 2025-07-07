using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using OpenFlowWebServer.Data;
using OpenFlowWebServer.Data.Domain;
using File = OpenFlowWebServer.Data.Domain.File;

namespace OpenFlowWebServer.Data.Repositories
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

    // Repozytorium dla klasy Log
    public interface ILogRepository : IRepository<Log> { }

    public class LogRepository : Repository<Log>, ILogRepository
    {
        public LogRepository(ApplicationDbContext context) : base(context) { }
    }

    // Repozytorium dla klasy Hyperparameter
    public interface IHyperparameterRepository : IRepository<Hyperparameter> { }

    public class HyperparameterRepository : Repository<Hyperparameter>, IHyperparameterRepository
    {
        public HyperparameterRepository(ApplicationDbContext context) : base(context) { }
    }

    // Repozytorium dla klasy Dataset
    public interface IDatasetRepository : IRepository<Dataset> { }

    public class DatasetRepository : Repository<Dataset>, IDatasetRepository
    {
        public DatasetRepository(ApplicationDbContext context) : base(context) { }
    }

    // Repozytorium dla klasy Model
    public interface IModelRepository : IRepository<Model> { }

    public class ModelRepository : Repository<Model>, IModelRepository
    {
        public ModelRepository(ApplicationDbContext context) : base(context) { }

    }

    // Repozytorium dla klasy Project
    public interface IProjectRepository : IRepository<Project> { }

    public class ProjectRepository : Repository<Project>, IProjectRepository
    {
        public ProjectRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Project?> GetByIdAsync(object id)
        {
            var models = _context.Models.Where(m => m.ProjectId == (Guid)id).ToListAsync();
            var datasets = _context.Datasets.Where(m => m.ProjectId == (Guid)id).ToListAsync();
            var project = await _dbSet.FirstOrDefaultAsync(p => p.Id == (Guid)id);
            Task.WaitAll(models, datasets);
            project.Models = models.Result;
            project.Datasets = datasets.Result;
            /*return await _dbSet
                .Include(p => p.Models)
                .FirstOrDefaultAsync(p => p.Id == (Guid)id);*/
            return project;
        }
    }

    // Repozytorium dla klasy Device
    public interface IDeviceRepository : IRepository<Device> { }

    public class DeviceRepository : Repository<Device>, IDeviceRepository
    {
        public DeviceRepository(ApplicationDbContext context) : base(context) { }
    }

    public interface IFileRepository : IRepository<File> { }
    public class FileRepository : Repository<File>, IFileRepository
    {
        public FileRepository(ApplicationDbContext context) : base(context) { }
    }
}
