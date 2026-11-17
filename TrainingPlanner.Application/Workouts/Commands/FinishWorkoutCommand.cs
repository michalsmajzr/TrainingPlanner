using Microsoft.EntityFrameworkCore;
using TrainingPlanner.Application.Abstractions;
using TrainingPlanner.Domain.Workouts.Enums;

namespace TrainingPlanner.Application.Workouts.Commands;

public sealed record FinishWorkoutCommand(Guid WorkoutId, string UserId);

public sealed class FinishWorkoutCommandHandler(IApplicationDbContext dbContext) : ICommandHandler<FinishWorkoutCommand, bool>
{
    public async Task<Result<bool>> Handle(FinishWorkoutCommand command, CancellationToken cancellationToken)
    {
        var workout = await dbContext.Workouts
            .FirstOrDefaultAsync(x => x.Id == command.WorkoutId && x.UserId == command.UserId, cancellationToken);

        if (workout is null)
        {
            return Result<bool>.Failure(new Error("workout.not_found", "Workout was not found."));
        }

        if (workout.Status == WorkoutStatus.Completed)
        {
            return Result<bool>.Success(true);
        }

        if (workout.Status != WorkoutStatus.InProgress)
        {
            return Result<bool>.Failure(new Error("workout.not_in_progress", "Workout is not in progress."));
        }

        workout.Status = WorkoutStatus.Completed;
        workout.FinishedAt = DateTimeOffset.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
