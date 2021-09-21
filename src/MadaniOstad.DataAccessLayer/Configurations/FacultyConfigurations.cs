using MadaniOstad.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MadaniOstad.DataAccessLayer.Configurations
{
    public class FacultyConfigurations : IEntityTypeConfiguration<Faculty>
    {
        public void Configure(EntityTypeBuilder<Faculty> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name).IsRequired();

            builder.HasIndex(e => e.Slug).IsUnique();
            builder.Property(e => e.Slug).IsRequired();
        }
    }
}
