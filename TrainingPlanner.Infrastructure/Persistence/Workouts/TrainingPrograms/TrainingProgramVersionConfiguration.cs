using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingPlanner.Domain.Workouts.TrainingPrograms;

namespace TrainingPlanner.Infrastructure.Persistence.Workouts.TrainingPrograms;

public class TrainingProgramVersionConfiguration : IEntityTypeConfiguration<TrainingProgramVersion>
{
    public void Configure(EntityTypeBuilder<TrainingProgramVersion> builder)
    {
        builder.ToTable("training_program_version");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TrainingType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.HasIndex(x => new
        {
            x.TrainingProgramId,
            x.VersionNumber
        }).IsUnique();

        builder.HasMany(x => x.Days)
            .WithOne(x => x.TrainingProgramVersion)
            .HasForeignKey(x => x.TrainingProgramVersionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
