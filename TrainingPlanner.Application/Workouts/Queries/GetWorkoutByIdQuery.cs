using Microsoft.EntityFrameworkCore;
using TrainingPlanner.Application.Abstractions;
using TrainingPlanner.Domain.Workouts.Enums;

namespace TrainingPlanner.Application.Workouts.Queries;

public sealed record GetWorkoutByIdQuery(Guid Id, string UserId);

public sealed record WorkoutDto(
    Guid Id,
    DateTimeOffset StartedAt,
    DateTimeOffset? FinishedAt,
    WorkoutStatus Status,
    IReadOnlyList<WorkoutExerciseDto> Exercises);

public sealed record WorkoutExerciseDto(
    Guid Id,
    Guid ExerciseId,
    string Name,
    int Position,
    IReadOnlyList<WorkoutSetDto> Sets,
    bool IsDefault);

public sealed record WorkoutSetDto(
    Guid Id,
    int Position,
    decimal? WeightKg,
    int? Repetitions);

public sealed class GetWorkoutByIdQueryHandler(IApplicationDbContext dbContext) : IQueryHandler<GetWorkoutByIdQuery, WorkoutDto?>
{
    public async Task<WorkoutDto?> Handle(GetWorkoutByIdQuery query, CancellationToken cancellationToken)
    {
        var workout = await dbContext.Workouts
            .AsNoTracking()
            .Include(x => x.Exercises)
                .ThenInclude(x => x.Exercise)
            .Include(x => x.Exercises)
                .ThenInclude(x => x.Sets)
            .FirstOrDefaultAsync(x => x.Id == query.Id && x.UserId == query.UserId, cancellationToken);

        if (workout is null)
        {
            return null;
        }

        return new WorkoutDto(
            workout.Id,
            workout.StartedAt,
            workout.FinishedAt,
            workout.Status,
            workout.Exercises
                .OrderBy(x => x.Position)
                .Select(x => new WorkoutExerciseDto(
                    x.Id,
                    x.ExerciseId,
                    x.Exercise.Name,
                    x.Position,
                    x.Sets
                        .OrderBy(s => s.Position)
                        .Select(s => new WorkoutSetDto(
                            s.Id,
                            s.Position,
                            s.WeightKg,
                            s.Repetitions))
                        .ToList(),
                    x.Exercise.IsDefault))
                .ToList()
        );
    }
}