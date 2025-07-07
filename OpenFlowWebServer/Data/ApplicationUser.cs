using Microsoft.AspNetCore.Identity;
using MongoDbGenericRepository.Attributes;
using AspNetCore.Identity.Mongo.Model;
namespace OpenFlowWebServer.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    /*public class ApplicationUser : IdentityUser
    {
        /*Guid _userId;#1#
    }*/
    public class ApplicationUser : MongoUser
    {
        // Add any custom fields you want
    }
    public class ApplicationRole : MongoRole
    {
        // Custom role logic (optional)
    }
    /*[CollectionName("Users")]
    public class ApplicationUser : MongoUser<int>
    {
        // Add custom properties if needed
        // public Guid UserId { get; set; } = Guid.NewGuid();
        // public string? UserName { get; set; }
        // public string? Email { get; set; }
        // public string? PasswordHash { get; set; }
    }*/


    /*using AspNetCore.Identity.MongoDbCore.Models;
    using MongoDbGenericRepository.Attributes;#1#

    [CollectionName("Roles")]
    public class ApplicationRole : MongoIdentityRole<int>
    {
        // Add custom properties if needed
    }*/

}
