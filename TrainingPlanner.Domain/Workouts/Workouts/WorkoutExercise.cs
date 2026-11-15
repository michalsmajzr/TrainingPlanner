using TrainingPlanner.Domain.Workouts.Exercises;
using TrainingPlanner.Domain.Workouts.TrainingPrograms;

namespace TrainingPlanner.Domain.Workouts.Workouts;

public class WorkoutExercise
{
    public Guid Id { get; set; }

    public Guid WorkoutId { get; set; }
    public Guid ExerciseId { get; set; }

    public int Position { get; set; }

    public Guid? TrainingProgramDayExerciseId { get; set; }

    public Workout Workout { get; set; } = null!;
    public Exercise Exercise { get; set; } = null!;

    public TrainingProgramDayExercise? TrainingProgramDayExercise
    { get; set; }

    public ICollection<WorkoutSet> Sets
    { get; set; } = new List<WorkoutSet>();
}
