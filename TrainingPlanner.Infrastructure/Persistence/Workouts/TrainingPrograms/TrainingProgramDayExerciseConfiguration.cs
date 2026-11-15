using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingPlanner.Domain.Workouts.TrainingPrograms;

namespace TrainingPlanner.Infrastructure.Persistence.Workouts.TrainingPrograms;

public class TrainingProgramDayExerciseConfiguration : IEntityTypeConfiguration<TrainingProgramDayExercise>
{
    public void Configure(EntityTypeBuilder<TrainingProgramDayExercise> builder)
    {
        builder.ToTable("training_program_day_exercise");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.WeightOverrideKg)
            .HasPrecision(7, 2);

        builder.HasIndex(x => new
        {
            x.TrainingProgramDayId,
            x.Position
        }).IsUnique();

        builder.HasOne(x => x.Exercise)
            .WithMany()
            .HasForeignKey(x => x.ExerciseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
