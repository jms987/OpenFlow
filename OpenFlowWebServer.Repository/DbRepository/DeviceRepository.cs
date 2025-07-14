using OpenFlowWebServer.Domain.Entities;
using OpenFlowWebServer.Domain;
using OpenFlowWebServer.Repository.DbRepository;

public interface IDeviceRepository : IRepository<Device> { }

public class DeviceRepository : Repository<Device>, IDeviceRepository
{
    public DeviceRepository(ApplicationDbContext context) : base(context) { }
}