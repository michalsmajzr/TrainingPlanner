using TrainingPlanner.Domain.Workouts.TrainingPrograms;
using TrainingPlanner.Domain.Workouts.Enums;

namespace TrainingPlanner.Domain.Workouts.Workouts;

public class Workout
{
    public Guid Id { get; set; }
    public required string UserId { get; set; }

    public Guid? TrainingProgramDayId { get; set; }

    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? FinishedAt { get; set; }

    public WorkoutStatus Status { get; set; }

    public TrainingProgramDay? TrainingProgramDay { get; set; }

    public ICollection<WorkoutExercise> Exercises
    { get; set; } = new List<WorkoutExercise>();
}
