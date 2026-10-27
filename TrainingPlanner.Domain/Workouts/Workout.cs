
namespace TrainingPlanner.Domain.Workouts;

public class Workout
{
    public Guid Id { get; init; }
    public string Name { get; private set; } = string.Empty;
    public DateTime Date { get; private set; }

    public Workout(string name, DateTime date)
    {
        Id = Guid.NewGuid();
        Name = name.Trim();
        Date = date;
    }
}
