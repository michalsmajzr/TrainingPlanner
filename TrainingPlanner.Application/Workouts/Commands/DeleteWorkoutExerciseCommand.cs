using Microsoft.EntityFrameworkCore;
using TrainingPlanner.Application.Abstractions;

namespace TrainingPlanner.Application.Workouts.Commands;

public sealed record DeleteWorkoutExerciseCommand(
    Guid WorkoutId,
    Guid WorkoutExerciseId,
    string UserId);

public sealed class DeleteWorkoutExerciseCommandHandler(IApplicationDbContext dbContext) : ICommandHandler<DeleteWorkoutExerciseCommand, bool>
{
    public async Task<Result<bool>> Handle(DeleteWorkoutExerciseCommand command, CancellationToken cancellationToken)
    {
        var workoutExercise = await dbContext.WorkoutExercises
            .Include(x => x.Workout)
            .FirstOrDefaultAsync(
                        x => x.Id == command.WorkoutExerciseId 
                        && x.WorkoutId == command.WorkoutId
                        && x.Workout.UserId == command.UserId,
                cancellationToken);

        if (workoutExercise is null)
        {
            return Result<bool>.Failure(new Error("workout_exercise.not_found", "Workout exercise was not found."));
        }

        dbContext.WorkoutExercises.Remove(workoutExercise);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
