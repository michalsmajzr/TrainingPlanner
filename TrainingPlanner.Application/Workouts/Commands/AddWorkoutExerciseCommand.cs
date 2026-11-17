using Microsoft.EntityFrameworkCore;
using TrainingPlanner.Application.Abstractions;
using TrainingPlanner.Domain.Workouts.Enums;
using TrainingPlanner.Domain.Workouts.Workouts;

namespace TrainingPlanner.Application.Workouts.Commands;

public sealed record AddWorkoutExerciseCommand(Guid WorkoutId, string UserId, Guid ExerciseId);

public sealed class AddWorkoutExerciseCommandHandler(IApplicationDbContext dbContext) : ICommandHandler<AddWorkoutExerciseCommand, Guid>
{
    public async Task<Result<Guid>> Handle(AddWorkoutExerciseCommand command, CancellationToken cancellationToken)
    {
        var workout = await dbContext.Workouts
            .Include(x => x.Exercises)
            .FirstOrDefaultAsync(x => x.Id == command.WorkoutId && x.UserId == command.UserId, cancellationToken);

        if (workout is null)
        {
            return Result<Guid>.Failure(new Error("workout.not_found","Workout was not found."));
        }

        if (workout.Status != WorkoutStatus.InProgress)
        {
            return Result<Guid>.Failure(new Error("workout.not_in_progress", "Workout is no longer in progress."));
        };

        var exerciseExists = await dbContext.Exercises
            .AnyAsync(x => x.Id == command.ExerciseId, cancellationToken);

        if (!exerciseExists)
        {
            return Result<Guid>.Failure(new Error("exercise.not_found", "Exercise was not found."));
        }

        var position = workout.Exercises
            .Select(x => x.Position)
            .DefaultIfEmpty(0)
            .Max() + 1;

        var workoutExercise = new WorkoutExercise
        {
            Id = Guid.NewGuid(),
            WorkoutId = workout.Id,
            ExerciseId = command.ExerciseId,
            Position = position
        };

        dbContext.WorkoutExercises.Add(workoutExercise);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(workoutExercise.Id);
    }
}