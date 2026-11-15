using TrainingPlanner.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace TrainingPlanner.Application.Workouts.Queries;

public sealed record GetAllTrainingProgramsQuery;
public sealed record TrainingProgramListItemDto(Guid Id, string Name, DateTimeOffset CreatedAt);

public sealed class GetAllTrainingProgramsQueryHandler(IApplicationDbContext dbContext)
    : IQueryHandler<GetAllTrainingProgramsQuery, IReadOnlyList<TrainingProgramListItemDto>>
{
    public async Task<IReadOnlyList<TrainingProgramListItemDto>> Handle(GetAllTrainingProgramsQuery query, CancellationToken cancellationToken)
    {
        var trainingPrograms = await dbContext.TrainingPrograms
            .AsNoTracking()
            .OrderByDescending(trainingProgram => trainingProgram.CreatedAt)
            .Select(trainingProgram => new TrainingProgramListItemDto(trainingProgram.Id, trainingProgram.Name, trainingProgram.CreatedAt))
            .ToListAsync(cancellationToken);

        return trainingPrograms;
    }
}
