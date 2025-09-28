using Microsoft.EntityFrameworkCore;
using OpenFlowWebServer.Domain;
using OpenFlowWebServer.Domain.Entities;
using OpenFlowWebServer.Repository.DbRepository;

namespace OpenFlowWebServer.Repository.DbRepository
{
    public interface ISecurityRepository : IRepository<Security>
    {
    }

    public class SecurityRepository: Repository<Security>, ISecurityRepository
    {
        public SecurityRepository(ApplicationDbContext context) : base(context) { }
    }
}