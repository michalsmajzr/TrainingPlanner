using Microsoft.EntityFrameworkCore;
using TrainingPlanner.Application.Abstractions;

namespace TrainingPlanner.Application.Workouts.Commands;

public sealed record DeleteWorkoutCommand(
    Guid WorkoutId,
    string UserId);

public sealed class DeleteWorkoutCommandHandler(IApplicationDbContext dbContext) : ICommandHandler<DeleteWorkoutCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteWorkoutCommand command, CancellationToken cancellationToken)
    {
        var workout = await dbContext.Workouts
            .FirstOrDefaultAsync(
                x => x.Id == command.WorkoutId && x.UserId == command.UserId,
                cancellationToken);

        if (workout is null)
        {
            return Result<bool>.Failure(new Error("workout.not_found", "Workout was not found."));
        }

        dbContext.Workouts.Remove(workout);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
