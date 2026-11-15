namespace TrainingPlanner.Domain.Workouts.TrainingPrograms;

public class TrainingProgram
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public string Name { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }

    public Guid? CurrentVersionId { get; set; }

    public TrainingProgramVersion? CurrentVersion { get; set; }

    public ICollection<TrainingProgramVersion> Versions
    { get; set; } = new List<TrainingProgramVersion>();
}
