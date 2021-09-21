using MadaniOstad.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MadaniOstad.DataAccessLayer.Configurations
{
    public class UserConfigurations : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.PasswordHash).IsRequired();

            builder.HasIndex(e => e.Email).IsUnique();
            builder.Property(e => e.Email).IsRequired();
        }
    }
}
