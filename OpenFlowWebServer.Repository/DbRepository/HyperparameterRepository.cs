using OpenFlowWebServer.Domain.Entities;
using OpenFlowWebServer.Domain;
using OpenFlowWebServer.Repository.DbRepository;

public interface IHyperparameterRepository : IRepository<Hyperparameter> { }

public class HyperparameterRepository : Repository<Hyperparameter>, IHyperparameterRepository
{
    public HyperparameterRepository(ApplicationDbContext context) : base(context) { }
}