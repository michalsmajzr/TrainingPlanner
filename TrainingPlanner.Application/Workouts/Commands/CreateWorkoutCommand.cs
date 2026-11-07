using TrainingPlanner.Application.Abstractions;
using TrainingPlanner.Domain.Workouts;

namespace TrainingPlanner.Application.Workouts.Commands;

// Command to create a new workout
public sealed record CreateWorkoutCommand(string Name);
public sealed class CreateWorkoutCommandHandler(IApplicationDbContext dbContext) : ICommandHandler<CreateWorkoutCommand, Guid>
{
    // Handle the command to create a new workout
    public async Task<Result<Guid>> Handle(CreateWorkoutCommand command, CancellationToken cancellationToken)
    {
        // validate the command
        if (string.IsNullOrWhiteSpace(command.Name))
        {
            return Result<Guid>.Failure(new Error("Workout.InvalidName", "Title is required."));
        }

        var workout = new Workout(command.Name, DateTime.UtcNow);

        dbContext.Workouts.Add(workout);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(workout.Id);
    }
}
