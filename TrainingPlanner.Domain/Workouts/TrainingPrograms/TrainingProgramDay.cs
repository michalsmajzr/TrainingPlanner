namespace TrainingPlanner.Domain.Workouts.TrainingPrograms;

public class TrainingProgramDay
{
    public Guid Id { get; set; }
    public Guid TrainingProgramVersionId { get; set; }

    public string Name { get; set; } = string.Empty;
    public int Position { get; set; }

    public TrainingProgramVersion TrainingProgramVersion
    { get; set; } = null!;

    public ICollection<TrainingProgramDayExercise> Exercises
    { get; set; } = new List<TrainingProgramDayExercise>();
}
