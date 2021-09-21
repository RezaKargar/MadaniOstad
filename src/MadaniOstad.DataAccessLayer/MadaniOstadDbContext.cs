using MadaniOstad.Common.Utilities;
using MadaniOstad.Entities.Models;
using MadaniOstad.Entities.Models.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MadaniOstad.DataAccessLayer
{
    public class MadaniOstadDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {

        public MadaniOstadDbContext(DbContextOptions<MadaniOstadDbContext> options)
            : base(options)
        {
        } 
        
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<IAuditableBaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.Now;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifiedAt = DateTime.Now;
                        break;
                }
            }

            foreach (var entry in ChangeTracker.Entries().Where(x => x.Properties.Any(b => b.Metadata.Name == "IsDeleted")))
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.CurrentValues["IsDeleted"] = false;
                        break;
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.CurrentValues["IsDeleted"] = true;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Use reflection to find models and add them to model builder
            modelBuilder.RegisterAllEntities<IEntity>(typeof(IEntity).Assembly);

            // Add model configurations to model builder
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MadaniOstadDbContext).Assembly);

            // Make delete behavior for all relations in models to be RESTRICT
            modelBuilder.AddRestrictDeleteBehaviorConvention();

            // Make name of models to be plural (User => Users)
            modelBuilder.AddPluralizingTableNameConvention();
        }
    }
}
