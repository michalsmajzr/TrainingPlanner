using TrainingPlanner.Application.Abstractions;
using TrainingPlanner.Domain.Workouts.TrainingPrograms;

namespace TrainingPlanner.Application.Workouts.Commands;

public sealed record CreateTrainingProgramCommand(string UserId, string Name);
public sealed class CreateTrainingProgramCommandHandler(IApplicationDbContext dbContext) : ICommandHandler<CreateTrainingProgramCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateTrainingProgramCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
        {
            return Result<Guid>.Failure(new Error("TrainingProgram.InvalidName", "Title is required."));
        }

        var trainingProgram = new TrainingProgram
        {
            UserId = command.UserId,
            Name = command.Name,
            CreatedAt = DateTimeOffset.UtcNow
        };

        dbContext.TrainingPrograms.Add(trainingProgram);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(trainingProgram.Id);
    }
}
