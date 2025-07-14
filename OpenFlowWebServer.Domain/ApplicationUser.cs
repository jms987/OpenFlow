using AspNetCore.Identity.Mongo.Model;

namespace OpenFlowWebServer.Domain
{
    public class ApplicationUser : MongoUser
    {
        // Add any custom fields you want
    }
    public class ApplicationRole : MongoRole
    {
        // Custom role logic (optional)
    }


}
