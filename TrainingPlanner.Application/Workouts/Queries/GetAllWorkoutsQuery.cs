using TrainingPlanner.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
namespace TrainingPlanner.Application.Workouts.Queries;

public sealed record GetAllWorkoutsQuery(string UserId);

public sealed record WorkoutListItemDto(Guid Id, DateTimeOffset StartedAt, DateTimeOffset? FinishedAt, int ExerciseCount);

public sealed class GetAllWorkoutsQueryHandler(IApplicationDbContext dbContext)
    : IQueryHandler<GetAllWorkoutsQuery, IReadOnlyList<WorkoutListItemDto>>
{
    public async Task<IReadOnlyList<WorkoutListItemDto>> Handle(
        GetAllWorkoutsQuery query,
        CancellationToken cancellationToken)
    {
        return await dbContext.Workouts
            .AsNoTracking()
            .Where(workout => workout.UserId == query.UserId)
            .OrderByDescending(workout => workout.StartedAt)
            .Select(workout => new WorkoutListItemDto(
                    workout.Id,
                    workout.StartedAt,
                    workout.FinishedAt,
                    workout.Exercises.Count))
            .ToListAsync(cancellationToken);
    }
}
