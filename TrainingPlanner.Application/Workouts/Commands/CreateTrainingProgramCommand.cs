using TrainingPlanner.Application.Abstractions;
using TrainingPlanner.Domain.Workouts.TrainingPrograms;

namespace TrainingPlanner.Application.Workouts.Commands;

// Command to create a new workout
public sealed record CreateTrainingProgramCommand(string Name);
public sealed class CreateTrainingProgramCommandHandler(IApplicationDbContext dbContext) : ICommandHandler<CreateTrainingProgramCommand, Guid>
{
    // Handle the command to create a new workout
    public async Task<Result<Guid>> Handle(CreateTrainingProgramCommand command, CancellationToken cancellationToken)
    {
        // validate the command
        if (string.IsNullOrWhiteSpace(command.Name))
        {
            return Result<Guid>.Failure(new Error("TrainingProgram.InvalidName", "Title is required."));
        }

        var trainingProgram = new TrainingProgram
        {
            Name = command.Name,
            CreatedAt = DateTimeOffset.UtcNow
        };

        dbContext.TrainingPrograms.Add(trainingProgram);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(trainingProgram.Id);
    }
}
