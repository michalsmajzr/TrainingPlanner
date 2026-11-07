using Microsoft.EntityFrameworkCore;
using TrainingPlanner.Application.Abstractions;

namespace TrainingPlanner.Application.Workouts.Queries;

public sealed record GetWorkoutByIdQuery(Guid Id);
public sealed record WorkoutDto(Guid Id, string Name);

public sealed class GetWorkoutByIdQueryHandler(IApplicationDbContext dbContext)
    : IQueryHandler<GetWorkoutByIdQuery, WorkoutDto?>
{
    public async Task<WorkoutDto?> Handle(GetWorkoutByIdQuery query, CancellationToken cancellationToken)
    {
        return await dbContext.Workouts
            .AsNoTracking()
            .Where(workout => workout.Id == query.Id)
            .Select(workout => new WorkoutDto(workout.Id, workout.Name))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
