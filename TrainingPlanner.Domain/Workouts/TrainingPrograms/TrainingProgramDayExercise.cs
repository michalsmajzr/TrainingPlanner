namespace TrainingPlanner.Domain.Workouts.TrainingPrograms;

public class TrainingProgramDayExercise
{
    public Guid Id { get; set; }

    public Guid TrainingProgramDayId { get; set; }
    public Guid ExerciseId { get; set; }

    public int Position { get; set; }

    public decimal? WeightOverrideKg { get; set; }
    public int? RepetitionsOverride { get; set; }
    public int? SetsOverride { get; set; }

    public TrainingProgramDay TrainingProgramDay
    { get; set; } = null!;

    public Exercises.Exercise Exercise { get; set; } = null!;
}
