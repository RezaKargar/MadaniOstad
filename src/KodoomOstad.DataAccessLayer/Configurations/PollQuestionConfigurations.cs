using KodoomOstad.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KodoomOstad.DataAccessLayer.Configurations
{
    public class PollQuestionConfigurations : IEntityTypeConfiguration<PollQuestion>
    {
        public void Configure(EntityTypeBuilder<PollQuestion> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasIndex(e => e.Question).IsUnique();
            builder.Property(e => e.Question).IsRequired();

            builder.Property(e => e.MinScore).HasColumnType("smallint");

            builder.Property(e => e.MaxScore).HasColumnType("smallint");
        }
    }
}
