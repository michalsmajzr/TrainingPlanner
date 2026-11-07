using TrainingPlanner.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace TrainingPlanner.Application.Workouts.Queries;

public sealed record GetAllWorkoutsQuery;
public sealed record WorkoutListItemDto(Guid Id, string Name, DateTime Date);

public sealed class GetAllWorkoutsQueryHandler(IApplicationDbContext dbContext)
    : IQueryHandler<GetAllWorkoutsQuery, IReadOnlyList<WorkoutListItemDto>>
{
    public async Task<IReadOnlyList<WorkoutListItemDto>> Handle(GetAllWorkoutsQuery query, CancellationToken cancellationToken)
    {
        var workouts = await dbContext.Workouts
            .AsNoTracking()
            .OrderByDescending(workout => workout.Date)
            .Select(workout => new WorkoutListItemDto(workout.Id, workout.Name, workout.Date))
            .ToListAsync(cancellationToken);

        return workouts;
    }
}
