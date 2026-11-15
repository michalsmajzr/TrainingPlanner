using Microsoft.AspNetCore.Mvc;
using TrainingPlanner.Application.Abstractions;
using TrainingPlanner.Application.Workouts.Commands;
using TrainingPlanner.Application.Workouts.Queries;

namespace TrainingPlanner.Api.Controllers.Workouts;

[Route("api/workouts")]
public sealed class WorkoutsController(
    ICommandHandler<CreateTrainingProgramCommand, Guid> createWorkoutHandler,
    ICommandHandler<DeleteTrainingProgramCommand, bool> deleteWorkoutHandler,
    IQueryHandler<GetAllTrainingProgramsQuery, IReadOnlyList<TrainingProgramListItemDto>> getAllWorkoutsHandler,
    IQueryHandler<GetTrainingProgramIdQuery, TrainingProgramDto?> getWorkoutByIdHandler)
    : ApiControllerBase
{
    // POST api/workouts
    [HttpPost]
    public async Task<IActionResult> Create(CreateWorkoutRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateTrainingProgramCommand(request.Name);
        var result = await createWorkoutHandler.Handle(command, cancellationToken);

        if (result.IsFailure)
        {
            return ProblemResponse(
                statusCode: StatusCodes.Status400BadRequest,
                errorCode: result.Error.Code,
                detail: result.Error.Description,
                title: "Bad Request");
        }

        return Ok(new CreateWorkoutResponse(result.Value));
    }

    // GET api/workouts
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var workouts = await getAllWorkoutsHandler.Handle(new GetAllTrainingProgramsQuery(), cancellationToken);
        return Ok(workouts);
    }

    // GET api/workouts/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var workout = await getWorkoutByIdHandler.Handle(new GetTrainingProgramIdQuery(id), cancellationToken);

        if (workout is null)
        {
            return ProblemResponse(
                statusCode: StatusCodes.Status404NotFound,
                errorCode: "workout.not_found",
                title: "Not Found");
        }

        return Ok(workout);
    }

    // DELETE api/workouts/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await deleteWorkoutHandler.Handle(new DeleteTrainingProgramCommand(id), cancellationToken);

        if (result.IsFailure)
        {
            return ProblemResponse(
                statusCode: StatusCodes.Status404NotFound,
                errorCode: result.Error.Code,
                detail: result.Error.Description,
                title: "Not Found");
        }

        return NoContent();
    }
}

// DTO for creating a workout
public sealed record CreateWorkoutRequest(string Name);
public sealed record CreateWorkoutResponse(Guid Id);