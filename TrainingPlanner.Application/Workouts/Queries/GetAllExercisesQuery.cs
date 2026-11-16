using TrainingPlanner.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace TrainingPlanner.Application.Workouts.Queries;

public sealed record GetAllExercisesQuery;

public sealed record ExerciseListItemDto(Guid Id, string Name, string MuscleGroup, string? Equipment, bool IsDefault);

public sealed class GetAllExercisesQueryHandler(IApplicationDbContext dbContext)
    : IQueryHandler<GetAllExercisesQuery, IReadOnlyList<ExerciseListItemDto>>
{
    public async Task<IReadOnlyList<ExerciseListItemDto>> Handle(GetAllExercisesQuery query, CancellationToken cancellationToken)
    {
        var exercises = await dbContext.Exercises
            .AsNoTracking()
            .OrderBy(exercise => exercise.Name)
            .Select(exercise => new ExerciseListItemDto(
                exercise.Id,
                exercise.Name,
                exercise.MuscleGroup,
                exercise.Equipment,
                exercise.IsDefault))
            .ToListAsync(cancellationToken);
        return exercises;
    }
}
