using Microsoft.AspNetCore.Mvc;
using TrainingPlanner.Application.Abstractions;
using TrainingPlanner.Application.Workouts.Queries;

namespace TrainingPlanner.Api.Controllers.Workouts;

[Route("api/exercises")]
public sealed class ExercisesController(
    IQueryHandler<GetAllExercisesQuery, IReadOnlyList<ExerciseListItemDto>> getAllExercisesHandler) 
    : ApiControllerBase
{
    // GET api/workouts
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var exercises = await getAllExercisesHandler.Handle(new GetAllExercisesQuery(), cancellationToken);
        return Ok(exercises);
    }
}
