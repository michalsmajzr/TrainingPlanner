using Microsoft.EntityFrameworkCore;
using TrainingPlanner.Domain.Workouts.Exercises;

namespace TrainingPlanner.Infrastructure.Persistence.Seed;

public static class ExerciseSeeder
{
    private sealed record DefaultExercise(string Name, string MuscleGroup, string? Equipment);

    private static readonly DefaultExercise[] DefaultExercises =
    [
        new("squat", "legs", "barbell"),
        new("pushUp", "chest", null),
        new("deadlift", "back", "barbell"),
        new("benchPress", "chest", "barbell"),
        new("pullUp", "back", "pullUpBar"),
        new("overheadPress", "shoulders", "barbell"),

        new("wideGripLatPulldown", "back", "cableMachine"),
        new("seatedCableRow", "back", "cableMachine"),
        new("standingDumbbellLateralRaise", "shoulders", "dumbbells"),
        new("rotatingDumbbellCurl", "biceps", "dumbbells"),
        new("tricepsPushdown", "triceps", "cableMachine"),

        new("inclineDumbbellBenchPress", "chest", "dumbbells"),
        new("barbellBentOverRow", "back", "barbell"),
        new("dips", "chest", "dipBars"),
        new("bentOverDumbbellReverseFly", "shoulders", "dumbbells"),
        new("preacherCurl", "biceps", "preacherBench"),

        new("legPress", "legs", "legPressMachine"),
        new("dumbbellRomanianDeadlift", "legs", "dumbbells"),
        new("legExtension", "legs", "legExtensionMachine"),
        new("standingCalfRaise", "legs", "calfRaiseMachine"),
        new("hangingLegRaise", "abs", "pullUpBar"),

        new("hackSquat", "legs", "hackSquatMachine"),
        new("legCurl", "legs", "legCurlMachine"),
        new("seatedCalfRaise", "legs", "seatedCalfRaiseMachine"),
        new("sitUp", "abs", null)
    ];

    public static async Task SeedAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken = default)
    {
        var existingNames = await dbContext.Exercises
            .Where(exercise => exercise.IsDefault)
            .Select(exercise => exercise.Name)
            .ToHashSetAsync(cancellationToken);

        var missingExercises = DefaultExercises
            .Where(exercise => !existingNames.Contains(exercise.Name))
            .Select(exercise => new Exercise
            {
                Id = Guid.NewGuid(),
                Name = exercise.Name,
                MuscleGroup = exercise.MuscleGroup,
                Equipment = exercise.Equipment,
                IsDefault = true
            })
            .ToArray();

        if (missingExercises.Length == 0)
        {
            return;
        }

        dbContext.Exercises.AddRange(missingExercises);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}