using Microsoft.EntityFrameworkCore;
using OpenFlowWebServer.Domain;
using OpenFlowWebServer.Domain.Entities;
using OpenFlowWebServer.Repository.DbRepository;

public interface IModelRepository : IRepository<Model> { }

public class ModelRepository : Repository<Model>, IModelRepository
{
    public ModelRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Model?> GetByIdAsync(object id)
    {
        var model = _dbSet.FirstOrDefault(x => x.Id == (Guid)id);
        model.Hyperparameters = _context.Hyperparameter.Where(h => h.ModelId == (Guid)id).ToList();
        return model;
    }
}