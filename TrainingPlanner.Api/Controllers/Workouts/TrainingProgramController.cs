using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrainingPlanner.Application.Abstractions;
using TrainingPlanner.Application.Workouts.Commands;
using TrainingPlanner.Application.Workouts.Queries;

namespace TrainingPlanner.Api.Controllers.Workouts;

[Route("api/training-programs")]
public sealed class TrainingProgramsController(
    ICommandHandler<CreateTrainingProgramCommand, Guid> createTrainingProgramHandler,
    ICommandHandler<DeleteTrainingProgramCommand, bool> deleteTrainingProgramHandler,
    IQueryHandler<GetAllTrainingProgramsQuery, IReadOnlyList<TrainingProgramListItemDto>> getAllTrainingProgramsHandler,
    IQueryHandler<GetTrainingProgramIdQuery, TrainingProgramDto?> getTrainingProgramByIdHandler)
    : ApiControllerBase
{
    // POST api/training-programs
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateTrainingProgramRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var command = new CreateTrainingProgramCommand(userId, request.Name);
        var result = await createTrainingProgramHandler.Handle(command, cancellationToken);

        if (result.IsFailure)
        {
            return ProblemResponse(
                statusCode: StatusCodes.Status400BadRequest,
                errorCode: result.Error.Code,
                detail: result.Error.Description,
                title: "Bad Request");
        }

        return Ok(new CreateTrainingProgramResponse(result.Value));
    }

    // GET api/training-programs
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var trainingPrograms = await getAllTrainingProgramsHandler.Handle(
            new GetAllTrainingProgramsQuery(),
            cancellationToken);

        return Ok(trainingPrograms);
    }

    // GET api/training-programs/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var trainingProgram = await getTrainingProgramByIdHandler.Handle(
            new GetTrainingProgramIdQuery(id),
            cancellationToken);

        if (trainingProgram is null)
        {
            return ProblemResponse(
                statusCode: StatusCodes.Status404NotFound,
                errorCode: "training_program.not_found",
                title: "Not Found");
        }

        return Ok(trainingProgram);
    }

    // DELETE api/training-programs/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await deleteTrainingProgramHandler.Handle(
            new DeleteTrainingProgramCommand(id),
            cancellationToken);

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

public sealed record CreateTrainingProgramRequest(string Name);
public sealed record CreateTrainingProgramResponse(Guid Id);
