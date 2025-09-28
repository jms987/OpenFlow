using Microsoft.EntityFrameworkCore;
using OpenFlowWebServer.Domain.Entities;
using File = OpenFlowWebServer.Domain.Entities.File;

namespace OpenFlowWebServer.Domain
{

    public class ApplicationDbContext :DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;
        }

        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Dataset> Datasets { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Hyperparameter> Hyperparameter { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<Security> Securities { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>(proj =>
                {
                    proj.OwnsMany(p => p.Models, mdl =>
                    {
                        // optional: configure key or property names
                        mdl.WithOwner().HasForeignKey("ProjectId");
                        mdl.Property<Guid>("Id");         // shadow key
                        mdl.HasKey("Id");
                    });
                });

                modelBuilder.Entity<Hyperparameter>()
                    .HasOne(h => h.Model)
                    .WithMany(m => m.Hyperparameters) // Adjust if it's a one-to-one relationship
                    .HasForeignKey(h => h.ModelId);

            base.OnModelCreating(modelBuilder);

        }

    }

}
