using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingPlanner.Domain.Workouts.Workouts;

namespace TrainingPlanner.Infrastructure.Persistence.Workouts.Workouts;

public class WorkoutSetConfiguration: IEntityTypeConfiguration<WorkoutSet>
{
    public void Configure(EntityTypeBuilder<WorkoutSet> builder)
    {
        builder.ToTable("workout_set");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TargetWeightKg)
            .HasPrecision(7, 2);

        builder.Property(x => x.WeightKg)
            .HasPrecision(7, 2);

        builder.HasIndex(x => new
        {
            x.WorkoutExerciseId,
            x.Position
        }).IsUnique();
    }
}
