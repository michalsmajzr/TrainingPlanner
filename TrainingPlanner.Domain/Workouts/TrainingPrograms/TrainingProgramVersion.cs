using TrainingPlanner.Domain.Workouts.Enums;

namespace TrainingPlanner.Domain.Workouts.TrainingPrograms;

public class TrainingProgramVersion
{
    public Guid Id { get; set; }
    public Guid TrainingProgramId { get; set; }

    public int VersionNumber { get; set; }
    public TrainingType TrainingType { get; set; }
    public int WorkoutsPerWeek { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public TrainingProgram TrainingProgram { get; set; } = null!;

    public ICollection<TrainingProgramDay> Days
    { get; set; } = new List<TrainingProgramDay>();
}
