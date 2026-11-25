using Microsoft.EntityFrameworkCore;
using TrainingPlanner.Application.Abstractions;
using TrainingPlanner.Domain.Workouts.Enums;

namespace TrainingPlanner.Application.Workouts.Commands;

public sealed record RemoveWorkoutExerciseCommand(
    Guid WorkoutId,
    Guid WorkoutExerciseId,
    string UserId);

public sealed class RemoveWorkoutExerciseCommandHandler(IApplicationDbContext dbContext) : ICommandHandler<RemoveWorkoutExerciseCommand, bool>
{
    public async Task<Result<bool>> Handle(RemoveWorkoutExerciseCommand command, CancellationToken cancellationToken)
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
