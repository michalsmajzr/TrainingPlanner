using TrainingPlanner.Application.Abstractions;
using TrainingPlanner.Domain.Workouts.Enums;
using TrainingPlanner.Domain.Workouts.Workouts;

namespace TrainingPlanner.Application.Workouts.Commands;
public sealed record CreateWorkoutCommand(string UserId);
public sealed class CreateWorkoutCommandHandler(IApplicationDbContext dbContext) : ICommandHandler<CreateWorkoutCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateWorkoutCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.UserId))
        {
            return Result<Guid>.Failure(new Error("workout.invalid_user", "User ID is required."));
        }

        var workout = new Workout
        {
            Id = Guid.NewGuid(),
            UserId = command.UserId,
            StartedAt = DateTimeOffset.UtcNow,
            FinishedAt = null,
            Status = WorkoutStatus.InProgress
        };

        dbContext.Workouts.Add(workout);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(workout.Id);
    }
}
