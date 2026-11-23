using Microsoft.EntityFrameworkCore;
using TrainingPlanner.Domain.Workouts.Exercises;
using TrainingPlanner.Domain.Workouts.Workouts;

namespace TrainingPlanner.Application.Abstractions;

public interface IApplicationDbContext
{
    DbSet<Workout> Workouts { get; }
    DbSet<WorkoutExercise> WorkoutExercises { get; }
    DbSet<Exercise> Exercises { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
