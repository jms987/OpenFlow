using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using OpenFlowWebServer.Data.Domain;
using File = OpenFlowWebServer.Data.Domain.File;

namespace OpenFlowWebServer.Data
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


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /*modelBuilder.Entity<Device>()
                .HasMany(d => d.Logs)
                .WithOne(l => l.Device)
                ;*/

                /*.WithOne(p=>p.Mro)*/
                /*modelBuilder.Entity<Project>()
                    .HasMany(p => p.Models)
                    ;*/
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

            /*
            modelBuilder.Model.HasOne(d => d.User)
                .WithMany(u => u.Devices)
                .HasForeignKey(d => d.UserId);
                */



            base.OnModelCreating(modelBuilder);

            /*modelBuilder.Entity<ApplicationUser>();*/
        }

        /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseMongoDB("mongodb://localhost:8002", "OpenFlowWebServer");

            base.OnConfiguring(optionsBuilder);
        }*/

        
    }

}
