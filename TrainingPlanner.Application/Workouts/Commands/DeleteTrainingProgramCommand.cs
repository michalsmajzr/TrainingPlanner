using Microsoft.EntityFrameworkCore;
using TrainingPlanner.Application.Abstractions;

namespace TrainingPlanner.Application.Workouts.Commands;

public sealed record DeleteTrainingProgramCommand(Guid Id);

public sealed class DeleteTrainingProgramCommandHandler(IApplicationDbContext dbContext) : ICommandHandler<DeleteTrainingProgramCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteTrainingProgramCommand command, CancellationToken cancellationToken)
    {
        var trainingProgram = await dbContext.TrainingPrograms
            .FirstOrDefaultAsync(w => w.Id == command.Id, cancellationToken);

        if (trainingProgram is null)
        {
            return Result<bool>.Failure(new Error("training_program.not_found", "TrainingProgram was not found."));
        }

        dbContext.TrainingPrograms.Remove(trainingProgram);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
