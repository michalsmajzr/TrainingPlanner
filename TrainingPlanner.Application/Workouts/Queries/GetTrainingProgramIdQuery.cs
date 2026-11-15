using Microsoft.EntityFrameworkCore;
using TrainingPlanner.Application.Abstractions;

namespace TrainingPlanner.Application.Workouts.Queries;

public sealed record GetTrainingProgramIdQuery(Guid Id);
public sealed record TrainingProgramDto(Guid Id, string Name);

public sealed class GetTrainingProgramByIdQueryHandler(IApplicationDbContext dbContext)
    : IQueryHandler<GetTrainingProgramIdQuery, TrainingProgramDto?>
{
    public async Task<TrainingProgramDto?> Handle(GetTrainingProgramIdQuery query, CancellationToken cancellationToken)
    {
        return await dbContext.TrainingPrograms
            .AsNoTracking()
            .Where(trainingProgram => trainingProgram.Id == query.Id)
            .Select(trainingProgram => new TrainingProgramDto(trainingProgram.Id, trainingProgram.Name))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
