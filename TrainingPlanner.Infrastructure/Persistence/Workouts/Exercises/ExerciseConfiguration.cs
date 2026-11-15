using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingPlanner.Domain.Workouts.Exercises;

namespace TrainingPlanner.Infrastructure.Persistence.Workouts.Exercises;

public class ExerciseConfiguration : IEntityTypeConfiguration<Exercise>
{
    public void Configure(EntityTypeBuilder<Exercise> builder)
    {
        builder.ToTable("exercise");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.MuscleGroup)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Equipment)
            .HasMaxLength(100);
    }
}
