using Microsoft.EntityFrameworkCore;
using TrainingPlanner.Domain.Workouts.TrainingPrograms;
using TrainingPlanner.Domain.Workouts.Workouts;

namespace TrainingPlanner.Application.Abstractions;

public interface IApplicationDbContext
{
    DbSet<TrainingProgram> TrainingPrograms { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
