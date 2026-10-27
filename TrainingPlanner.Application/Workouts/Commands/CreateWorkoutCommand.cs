using TrainingPlanner.Application.Abstractions;
using TrainingPlanner.Domain.Workouts;

namespace TrainingPlanner.Application.Workouts.Commands;

// Command to create a new workout
public sealed record CreateWorkoutCommand(string Name, DateTime Date);

public sealed class CreateWorkoutCommandHandler : ICommandHandler<CreateWorkoutCommand, Guid>
{
    // Inject the workout repository to handle data persistence
    private readonly IWorkoutRepository _workoutRepository;

    public CreateWorkoutCommandHandler(IWorkoutRepository workoutRepository)
    {
        _workoutRepository = workoutRepository;
    }

    // Handle the command to create a new workout
    public async Task<Result<Guid>> Handle(CreateWorkoutCommand command, CancellationToken cancellationToken)
    {
        // validate the command
        if (string.IsNullOrWhiteSpace(command.Name))
        {
            return Result<Guid>.Failure(new Error("Workout.InvalidName", "Název tréninku je povinný."));
        }

        var workout = new Workout(command.Name, command.Date);

        await _workoutRepository.AddAsync(workout, cancellationToken);

        return Result<Guid>.Success(workout.Id);
    }
}