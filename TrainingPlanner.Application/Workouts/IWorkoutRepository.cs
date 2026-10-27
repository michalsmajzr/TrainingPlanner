using TrainingPlanner.Domain.Workouts;

namespace TrainingPlanner.Application.Workouts;

public interface IWorkoutRepository
{
    Task AddAsync(Workout workout, CancellationToken cancellationToken);
    Task<IReadOnlyList<Workout>> GetAllAsync(CancellationToken cancellationToken);
}
