using Microsoft.EntityFrameworkCore;
using TrainingPlanner.Domain.Workouts;

namespace TrainingPlanner.Application.Abstractions;

public interface IApplicationDbContext
{
    DbSet<Workout> Workouts { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
