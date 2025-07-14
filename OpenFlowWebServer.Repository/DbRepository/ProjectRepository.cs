// Repozytorium dla klasy Project
using Microsoft.EntityFrameworkCore;
using OpenFlowWebServer.Domain;
using OpenFlowWebServer.Domain.Entities;
using OpenFlowWebServer.Repository.DbRepository;

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