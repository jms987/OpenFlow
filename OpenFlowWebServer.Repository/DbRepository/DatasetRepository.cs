using OpenFlowWebServer.Domain;
using OpenFlowWebServer.Domain.Entities;
using OpenFlowWebServer.Repository.DbRepository;

public interface IDatasetRepository : IRepository<Dataset> { }

public class DatasetRepository : Repository<Dataset>, IDatasetRepository
{
    public DatasetRepository(ApplicationDbContext context) : base(context) { }
}