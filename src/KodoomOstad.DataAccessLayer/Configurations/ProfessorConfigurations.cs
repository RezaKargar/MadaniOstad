using KodoomOstad.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KodoomOstad.DataAccessLayer.Configurations
{
    public class ProfessorConfigurations : IEntityTypeConfiguration<Professor>
    {
        public void Configure(EntityTypeBuilder<Professor> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name).IsRequired();

            builder.Property(e => e.Slug).IsRequired();

            builder.Property(e => e.Rank).HasColumnType("tinyint").IsRequired();

            builder.Property(e => e.AverageRate).HasColumnType("float");
        }
    }
}
