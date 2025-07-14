using OpenFlowWebServer.Domain;
using OpenFlowWebServer.Repository.DbRepository;
using File = OpenFlowWebServer.Domain.Entities.File;
public interface IFileRepository : IRepository<File> { }
public class FileRepository : Repository<File>, IFileRepository
{
    public FileRepository(ApplicationDbContext context) : base(context) { }
}