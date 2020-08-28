using Microsoft.EntityFrameworkCore;

namespace KodoomOstad.DataAccessLayer
{
    public class KodoomOstadDbContextFactory : DesignTimeDbContextFactoryBase<KodoomOstadDbContext>
    {
        protected override KodoomOstadDbContext CreateNewInstance(DbContextOptions<KodoomOstadDbContext> options)
        {
            return new KodoomOstadDbContext(options);
        }
    }
}
