using Microsoft.EntityFrameworkCore;
using TrainingPlanner.Domain.Workouts.TrainingPrograms;
using TrainingPlanner.Domain.Workouts.Exercises;

namespace TrainingPlanner.Application.Abstractions;

public interface IApplicationDbContext
{
    DbSet<TrainingProgram> TrainingPrograms { get; }
    DbSet<Exercise> Exercises { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
