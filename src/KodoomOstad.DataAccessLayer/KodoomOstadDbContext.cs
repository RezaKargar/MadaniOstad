using KodoomOstad.Common.Utilities;
using KodoomOstad.Entities.Models;
using KodoomOstad.Entities.Models.Base;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KodoomOstad.DataAccessLayer
{
    public class KodoomOstadDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {

        public KodoomOstadDbContext(DbContextOptions<KodoomOstadDbContext> options)
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

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Use reflection to find models and add them to model builder
            modelBuilder.RegisterAllEntities<IEntity>(typeof(IEntity).Assembly);

            // Add model configurations to model builder
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(KodoomOstadDbContext).Assembly);

            // Make delete behavior for all relations in models to be RESTRICT
            modelBuilder.AddRestrictDeleteBehaviorConvention();

            // Make name of models to be plural (User => Users)
            modelBuilder.AddPluralizingTableNameConvention();
        }
    }
}
