using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TrainingPlanner.Application.Abstractions;
using TrainingPlanner.Application.Workouts;
using TrainingPlanner.Application.Workouts.Commands;

namespace TrainingPlanner.Api.Controllers.Workouts;

[ApiController]
[Route("api/workouts")]
public sealed class WorkoutsController : ControllerBase
{
    private readonly ICommandHandler<CreateWorkoutCommand, Guid> _createWorkoutHandler;
    private readonly IWorkoutRepository _workoutRepository;

    // Inject the command handler for creating workouts
    public WorkoutsController(ICommandHandler<CreateWorkoutCommand, Guid> createWorkoutHandler, IWorkoutRepository workoutRepository)
    {
        _createWorkoutHandler = createWorkoutHandler;
        _workoutRepository = workoutRepository;
    }

    // POST api/workouts
    [HttpPost]
    public async Task<IActionResult> Create(CreateWorkoutRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateWorkoutCommand(request.Name, request.Date);
        var result = await _createWorkoutHandler.Handle(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(new { result.Error.Code, result.Error.Description });
        }

        return Ok(new CreateWorkoutResponse(result.Value));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var workouts = await _workoutRepository.GetAllAsync(cancellationToken);
        var response = workouts.Select(w => new WorkoutListItemResponse(w.Id, w.Name, w.Date)).ToList();
        return Ok(response);
    }
}

// Request model for creating a workout
public sealed record CreateWorkoutRequest(
    [Required, StringLength(100, MinimumLength = 1)] string Name,
    DateTime Date);

// Response model for creating a workout
public sealed record CreateWorkoutResponse(Guid Id);
// Response model for listing workouts
public sealed record WorkoutListItemResponse(Guid Id, string Name, DateTime Date);
