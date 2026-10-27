using Microsoft.EntityFrameworkCore;
using TrainingPlanner.Application.Workouts;
using TrainingPlanner.Domain.Workouts;

namespace TrainingPlanner.Infrastructure.Persistence.Workouts;

public sealed class WorkoutRepository : IWorkoutRepository
{
    private readonly ApplicationDbContext _dbContext;

    public WorkoutRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Add a new workout to the database
    public async Task AddAsync(Workout workout, CancellationToken cancellationToken)
    {
        await _dbContext.Set<Workout>().AddAsync(workout, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    // Get all workouts from the database, ordered by date descending
    public async Task<IReadOnlyList<Workout>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Set<Workout>()
            .AsNoTracking()
            .OrderByDescending(x => x.Date)
            .ToListAsync(cancellationToken);
    }
}
