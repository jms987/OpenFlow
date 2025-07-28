using Azure.Storage.Blobs.Models;
using Microsoft.EntityFrameworkCore;
using OpenFlowWebServer.Domain;
using OpenFlowWebServer.Domain.Entities;
using OpenFlowWebServer.Repository.DbRepository;

public interface IDatasetRepository : IRepository<Dataset> { }

public class DatasetRepository : Repository<Dataset>, IDatasetRepository
{
    public DatasetRepository(ApplicationDbContext context) : base(context)
    { }

    public override async Task<Dataset?> GetByIdAsync(object id)
    {
        var dataset = await _dbSet
            .FirstOrDefaultAsync(d => d.Id == (Guid)id);

        if (dataset == null)
            return null;

        var configFile = await _context.Files
            .FirstOrDefaultAsync(f => f.Id == dataset.ConfigFileId);
        if (configFile == null)
            throw new InvalidOperationException($"Config file with ID {dataset.ConfigFileId} not found for dataset {dataset.Id}.");

        var datasetFile = await _context.Files.
            FirstOrDefaultAsync(f => f.Id == dataset.DatasetFileId);
        if (datasetFile == null)
            throw new InvalidOperationException($"Dataset file with ID {dataset.DatasetFileId} not found for dataset {dataset.Id}.");

        dataset.ConfigFile = configFile;
        dataset.DatasetFile = datasetFile;

        return dataset;
    }
    
    public override async Task AddAsync(Dataset entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity), "Dataset cannot be null");
        }
        if (entity.ConfigFile == null & entity.ConfigFileId == null)
        {
            throw new ArgumentNullException(nameof(entity.ConfigFile), "Config file cannot be null");
        }
        if (entity.DatasetFile == null & entity.DatasetFileId == null)
        {
            throw new ArgumentNullException(nameof(entity.DatasetFile), "Dataset file cannot be null");
        }

        await base.AddAsync(entity);
    }


}