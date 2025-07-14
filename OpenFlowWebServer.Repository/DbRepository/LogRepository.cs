using OpenFlowWebServer.Domain.Entities;
using OpenFlowWebServer.Domain;
using OpenFlowWebServer.Repository.DbRepository;

public interface ILogRepository : IRepository<Log> { }

public class LogRepository : Repository<Log>, ILogRepository
{
    public LogRepository(ApplicationDbContext context) : base(context) { }
}