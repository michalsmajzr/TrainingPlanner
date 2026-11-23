namespace TrainingPlanner.Domain.Workouts.Exercises;

public class Exercise
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string MuscleGroup { get; set; } = string.Empty;
    public string? Equipment { get; set; }
    public bool IsDefault { get; set; }
}
