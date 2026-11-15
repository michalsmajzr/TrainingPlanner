namespace TrainingPlanner.Domain.Workouts.Workouts;

public class WorkoutSet
{
    public Guid Id { get; set; }

    public Guid WorkoutExerciseId { get; set; }

    public int Position { get; set; }

    public decimal? TargetWeightKg { get; set; }
    public int? TargetRepetitions { get; set; }

    public decimal? ActualWeightKg { get; set; }
    public int? ActualRepetitions { get; set; }

    public WorkoutExercise WorkoutExercise
    { get; set; } = null!;
}
