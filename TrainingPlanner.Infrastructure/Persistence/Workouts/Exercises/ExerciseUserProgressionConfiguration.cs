using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingPlanner.Domain.Workouts.Exercises;

namespace TrainingPlanner.Infrastructure.Persistence.Workouts.Exercises;

public class ExerciseUserProgressionConfiguration : IEntityTypeConfiguration<ExerciseUserProgression>
{
    public void Configure(EntityTypeBuilder<ExerciseUserProgression> builder)
    {
        builder.ToTable("exercise_user_progression");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new
        {
            x.UserId,
            x.ExerciseId,
            x.TrainingType
        }).IsUnique();

        builder.Property(x => x.TrainingType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(x => x.RecommendedWeightKg)
            .HasPrecision(7, 2);

        builder.HasOne(x => x.Exercise)
            .WithMany(x => x.UserProgressions)
            .HasForeignKey(x => x.ExerciseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
