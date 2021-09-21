using MadaniOstad.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MadaniOstad.DataAccessLayer.Configurations
{
    public class CourseConfigurations : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Title).IsRequired();

            builder.Property(e => e.Year).HasColumnType("tinyint");

            builder.Property(e => e.Term).HasColumnType("tinyint");

            builder.Property(e => e.Grade).HasColumnType("decimal(5,2)");
        }
    }
}
