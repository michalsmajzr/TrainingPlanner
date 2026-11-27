using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Windows.Input;
using TrainingPlanner.Application.Abstractions;
using TrainingPlanner.Application.Workouts.Commands;
using TrainingPlanner.Application.Workouts.Queries;

namespace TrainingPlanner.Api.Controllers.Workouts;

[Route("api/workout")]
public sealed class WorkoutController(
    ICommandHandler<CreateWorkoutCommand, Guid> createWorkoutHandler,
    IQueryHandler<GetAllWorkoutsQuery, IReadOnlyList<WorkoutListItemDto>> getAllWorkoutsHandler,
    IQueryHandler<GetWorkoutByIdQuery, WorkoutDto?> getWorkoutByIdHandler,
    ICommandHandler<AddWorkoutExerciseCommand, Guid> addWorkoutExerciseHandler,
    ICommandHandler<FinishWorkoutCommand, bool> finishWorkoutHandler,
    ICommandHandler<DeleteWorkoutCommand, bool> deleteWorkoutHandler,
    ICommandHandler<DeleteWorkoutExerciseCommand, bool> deleteWorkoutExerciseHandler)
    : ApiControllerBase
{
    // POST api/workout
    [HttpPost]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var command = new CreateWorkoutCommand(userId);

        var result = await createWorkoutHandler.Handle(command,cancellationToken);
        if (result.IsFailure)
        {
            return ProblemResponse(
                statusCode: StatusCodes.Status400BadRequest,
                errorCode: result.Error.Code,
                detail: result.Error.Description,
                title: "Bad Request");
        }

        return StatusCode(StatusCodes.Status201Created, new CreateWorkoutResponse(result.Value));
    }

    // GET api/workout
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var workouts = await getAllWorkoutsHandler.Handle(new GetAllWorkoutsQuery(userId), cancellationToken);

        return Ok(workouts);
    }

    // GET api/workout/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var workout = await getWorkoutByIdHandler.Handle(new GetWorkoutByIdQuery(id, userId),cancellationToken);
        if (workout is null)
        {
            return ProblemResponse(
                statusCode: StatusCodes.Status404NotFound,
                errorCode: "workout.not_found",
                title: "Not Found");
        }

        return Ok(workout);
    }

    // POST api/workout/{id}/exercises
    [HttpPost("{id:guid}/exercises")]
    public async Task<IActionResult> AddExercise(Guid id, AddWorkoutExerciseRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var result = await addWorkoutExerciseHandler.Handle(new AddWorkoutExerciseCommand(id, userId, request.ExerciseId), cancellationToken);
        if (result.IsFailure)
        {
            var (statusCode, title) = result.Error.Code switch
            {
                "workout.not_found" or "exercise.not_found" => (StatusCodes.Status404NotFound, "Not Found"),
                "workout.not_in_progress" => (StatusCodes.Status409Conflict, "Conflict"),
                _ => (StatusCodes.Status400BadRequest, "Bad Request")
            };

            return ProblemResponse(
                statusCode: statusCode,
                errorCode: result.Error.Code,
                detail: result.Error.Description,
                title: title);
        }

        return StatusCode(
            StatusCodes.Status201Created,
            new AddWorkoutExerciseResponse(result.Value));
    }

    // POST api/workout/{id}/finish
    [HttpPost("{id:guid}/finish")]
    public async Task<IActionResult> Finish(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var result = await finishWorkoutHandler.Handle(new FinishWorkoutCommand(id, userId),cancellationToken);
        if (result.IsFailure)
        {
            var (statusCode, title) = result.Error.Code switch
            {
                "workout.not_found" => (StatusCodes.Status404NotFound, "Not Found"),
                "workout.not_in_progress" => (StatusCodes.Status409Conflict, "Conflict"),
                _ => (StatusCodes.Status400BadRequest, "Bad Request")
            };

            return ProblemResponse(
                statusCode: statusCode,
                errorCode: result.Error.Code,
                detail: result.Error.Description,
                title: title);
        }

        return NoContent();
    }

    // DELETE api/workout/{workoutId}
    [HttpDelete("{workoutId:guid}")]
    public async Task<IActionResult> DeleteWorkout(
        Guid workoutId,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var command = new DeleteWorkoutCommand(workoutId, userId);

        var result = await deleteWorkoutHandler.Handle(command, cancellationToken);

        if (result.IsFailure)
        {
            var (statusCode, title) = result.Error.Code switch
            {
                "workout.not_found" => (StatusCodes.Status404NotFound, "Not Found"),

                _ => (StatusCodes.Status400BadRequest, "Bad Request")
            };

            return ProblemResponse(
                statusCode: statusCode,
                errorCode: result.Error.Code,
                detail: result.Error.Description,
                title: title);
        }

        return NoContent();
    }

    // DELETE api/workout/{workoutId}/exercises/{workoutExerciseId}
    [HttpDelete("{workoutId:guid}/exercises/{workoutExerciseId:guid}")]
    public async Task<IActionResult> DeleteWorkoutExercise(
        Guid workoutId,
        Guid workoutExerciseId,
        CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var command = new DeleteWorkoutExerciseCommand(workoutId, workoutExerciseId,userId);

        var result = await deleteWorkoutExerciseHandler.Handle(command, cancellationToken);

        if (result.IsFailure)
        {
            var (statusCode, title) = result.Error.Code switch
            {
                "workout_exercise.not_found" => (StatusCodes.Status404NotFound, "Not Found"),

                _ => (StatusCodes.Status400BadRequest, "Bad Request")
            };

            return ProblemResponse(
                statusCode: statusCode,
                errorCode: result.Error.Code,
                detail: result.Error.Description,
                title: title);
        }

        return NoContent();
    }
}

public sealed record CreateWorkoutResponse(Guid Id);
public sealed record AddWorkoutExerciseRequest(Guid ExerciseId);
public sealed record AddWorkoutExerciseResponse(Guid Id);