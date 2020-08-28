using KodoomOstad.Entities.Models;
using KodoomOstad.Entities.Models.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KodoomOstad.DataAccessLayer
{
    public class KodoomOstadDbContext : DbContext
    {

        public KodoomOstadDbContext(DbContextOptions<KodoomOstadDbContext> options)
            : base(options)
        {
        }

        public DbSet<Answer> Answers { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<Faculty> Faculties { get; set; }

        public DbSet<PollQuestion> PollQuestions { get; set; }

        public DbSet<Professor> Professors { get; set; }

        public DbSet<User> Users { get; set; }


        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<AuditableBaseEntity>())
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
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(KodoomOstadDbContext).Assembly);
        }
    }
}
