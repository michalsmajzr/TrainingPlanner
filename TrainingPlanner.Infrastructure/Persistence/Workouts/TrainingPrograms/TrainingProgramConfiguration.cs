using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingPlanner.Domain.Workouts.TrainingPrograms;

namespace TrainingPlanner.Infrastructure.Persistence.Workouts.TrainingPrograms;

public class TrainingProgramConfiguration : IEntityTypeConfiguration<TrainingProgram>
{
    public void Configure(EntityTypeBuilder<TrainingProgram> builder)
    {
        builder.ToTable("training_program");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasIndex(x => x.UserId);

        builder.HasOne(x => x.CurrentVersion)
            .WithMany()
            .HasForeignKey(x => x.CurrentVersionId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(x => x.Versions)
            .WithOne(x => x.TrainingProgram)
            .HasForeignKey(x => x.TrainingProgramId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
