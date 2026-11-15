using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingPlanner.Domain.Workouts.TrainingPrograms;

namespace TrainingPlanner.Infrastructure.Persistence.Workouts.TrainingPrograms;

public class TrainingProgramDayConfiguration : IEntityTypeConfiguration<TrainingProgramDay>
{
    public void Configure(EntityTypeBuilder<TrainingProgramDay> builder)
    {
        builder.ToTable("training_program_day");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => new
        {
            x.TrainingProgramVersionId,
            x.Position
        }).IsUnique();

        builder.HasMany(x => x.Exercises)
            .WithOne(x => x.TrainingProgramDay)
            .HasForeignKey(x => x.TrainingProgramDayId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
