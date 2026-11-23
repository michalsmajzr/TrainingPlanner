using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingPlanner.Domain.Workouts.Workouts;

namespace TrainingPlanner.Infrastructure.Persistence.Workouts.Workouts;

public class WorkoutExerciseConfiguration : IEntityTypeConfiguration<WorkoutExercise>
{
    public void Configure(EntityTypeBuilder<WorkoutExercise> builder)
    {
        builder.ToTable("workout_exercise");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new
        {
            x.WorkoutId,
            x.Position
        }).IsUnique();

        builder.HasOne(x => x.Exercise)
            .WithMany()
            .HasForeignKey(x => x.ExerciseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Sets)
            .WithOne(x => x.WorkoutExercise)
            .HasForeignKey(x => x.WorkoutExerciseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
