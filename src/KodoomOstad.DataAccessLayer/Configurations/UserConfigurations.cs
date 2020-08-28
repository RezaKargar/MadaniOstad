using KodoomOstad.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KodoomOstad.DataAccessLayer.Configurations
{
    public class UserConfigurations : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name).IsRequired();

            builder.HasIndex(e => e.StudentId).IsUnique();
            builder.Property(e => e.StudentId).IsRequired();

            builder.HasIndex(e => e.Email).IsUnique();
            builder.Property(e => e.Email).IsRequired();

            builder.HasIndex(e => e.Phone).IsUnique();
            builder.Property(e => e.Phone).IsRequired();

            builder.Property(e => e.Password).IsRequired();

            builder.Property(e => e.Role).IsRequired();
        }
    }
}
