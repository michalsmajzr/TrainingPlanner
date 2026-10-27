using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingPlanner.Domain.Workouts;

namespace TrainingPlanner.Infrastructure.Persistence.Workouts;

public sealed class WorkoutConfiguration : IEntityTypeConfiguration<Workout>
{
    public void Configure(EntityTypeBuilder<Workout> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(100);

        builder.Property(x => x.Date)
            .HasColumnType("timestamp with time zone");
    }
}
