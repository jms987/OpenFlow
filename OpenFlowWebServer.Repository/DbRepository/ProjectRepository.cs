// Repozytorium dla klasy Project
using Microsoft.EntityFrameworkCore;
using OpenFlowWebServer.Domain;
using OpenFlowWebServer.Domain.Entities;
using OpenFlowWebServer.Repository.DbRepository;

public interface IProjectRepository : IRepository<Project>
{
    public Task DeployModel(Guid Id,int initialCount);
    public Task ResetProject(Guid Id);
    public Task<Project?> GetDeployedProject();
}

public class ProjectRepository : Repository<Project>, IProjectRepository
{
    public ProjectRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Project?> GetByIdAsync(object id)
    {
        var models = _context.Models.Where(m => m.ProjectId == (Guid)id).ToListAsync();
        var datasets = _context.Datasets.Where(m => m.ProjectId == (Guid)id).ToListAsync();
        var project = await _dbSet.FirstOrDefaultAsync(p => p.Id == (Guid)id);
        var securities = _context.Securities.Where(s => s.ProjectId == (Guid)id).ToListAsync();
        Task.WaitAll(models, datasets,securities);
        project.Models = models.Result;
        project.Datasets = datasets.Result;
        project.Securities = securities.Result;
        /*return await _dbSet
            .Include(p => p.Models)
            .FirstOrDefaultAsync(p => p.Id == (Guid)id);*/
        return project;
    }

    public async Task DeployModel(Guid Id, int initialCount)
    {
        var project = await _dbSet.FirstOrDefaultAsync(p => p.Id == Id);
        project.IsDeployed = true;
        project.InitialConfigurations = initialCount;
        _dbSet.Update(project);
        _context.SaveChangesAsync();
    }

    public async Task ResetProject(Guid Id)
    {
        var project = await _dbSet.FirstOrDefaultAsync(p => p.Id == Id);
        project.IsDeployed = false;
        project.InitialConfigurations = 0;
        _dbSet.Update(project);
        await _context.SaveChangesAsync();
    }

    public async Task<Project?> GetDeployedProject()
    {
        var project = await _dbSet.FirstOrDefaultAsync(p=>p.IsDeployed == true);
        if (!ReferenceEquals(project, null))
        {
            return await GetByIdAsync(project.Id);
        }
        return null;
    }
}