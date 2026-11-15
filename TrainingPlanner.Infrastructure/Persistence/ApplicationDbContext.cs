using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TrainingPlanner.Application.Abstractions;
using TrainingPlanner.Domain.Workouts.Exercises;
using TrainingPlanner.Domain.Workouts.TrainingPrograms;
using TrainingPlanner.Domain.Workouts.Workouts;
using TrainingPlanner.Infrastructure.Identity;

namespace TrainingPlanner.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<User>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Exercise> Exercises => Set<Exercise>();
    public DbSet<ExerciseUserProgression> ExerciseUserProgressions => Set<ExerciseUserProgression>();

    public DbSet<TrainingProgram> TrainingPrograms => Set<TrainingProgram>();
    public DbSet<TrainingProgramVersion> TrainingProgramVersions => Set<TrainingProgramVersion>();
    public DbSet<TrainingProgramDay> TrainingProgramDays => Set<TrainingProgramDay>();
    public DbSet<TrainingProgramDayExercise> TrainingProgramDayExercises => Set<TrainingProgramDayExercise>();

    public DbSet<Workout> Workouts => Set<Workout>();
    public DbSet<WorkoutExercise> WorkoutExercises => Set<WorkoutExercise>();
    public DbSet<WorkoutSet> WorkoutSets => Set<WorkoutSet>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
