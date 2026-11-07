using Microsoft.EntityFrameworkCore;
using TrainingPlanner.Application.Abstractions;

namespace TrainingPlanner.Application.Workouts.Commands;

public sealed record DeleteWorkoutCommand(Guid Id);

public sealed class DeleteWorkoutCommandHandler(IApplicationDbContext dbContext) : ICommandHandler<DeleteWorkoutCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteWorkoutCommand command, CancellationToken cancellationToken)
    {
        var workout = await dbContext.Workouts
            .FirstOrDefaultAsync(w => w.Id == command.Id, cancellationToken);

        if (workout is null)
        {
            return Result<bool>.Failure(new Error("Workout.NotFound", "Workout was not found."));
        }

        dbContext.Workouts.Remove(workout);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
