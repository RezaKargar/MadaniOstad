using Microsoft.EntityFrameworkCore;

namespace MadaniOstad.DataAccessLayer
{
    public class MadaniOstadDbContextFactory : DesignTimeDbContextFactoryBase<MadaniOstadDbContext>
    {
        protected override MadaniOstadDbContext CreateNewInstance(DbContextOptions<MadaniOstadDbContext> options)
        {
            return new MadaniOstadDbContext(options);
        }
    }
}
