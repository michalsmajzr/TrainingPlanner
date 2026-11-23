using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingPlanner.Domain.Workouts.Workouts;

namespace TrainingPlanner.Infrastructure.Persistence.Workouts.Workouts;

public class WorkoutConfiguration : IEntityTypeConfiguration<Workout>
{
    public void Configure(EntityTypeBuilder<Workout> builder)
    {
        builder.ToTable("workout");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.HasIndex(x => new
        {
            x.UserId,
            x.StartedAt
        });

        builder.HasMany(x => x.Exercises)
            .WithOne(x => x.Workout)
            .HasForeignKey(x => x.WorkoutId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
