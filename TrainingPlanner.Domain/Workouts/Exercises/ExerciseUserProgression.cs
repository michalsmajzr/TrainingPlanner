using TrainingPlanner.Domain.Workouts.Enums;

namespace TrainingPlanner.Domain.Workouts.Exercises;

public class ExerciseUserProgression
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ExerciseId { get; set; }

    public TrainingType TrainingType { get; set; }

    public decimal RecommendedWeightKg { get; set; }
    public int RecommendedRepetitions { get; set; }
    public int RecommendedSets { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public Exercise Exercise { get; set; } = null!;
}
