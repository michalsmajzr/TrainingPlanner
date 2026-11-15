using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingPlanner.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Workouts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_training_program_day_training_program_TrainingProgramId",
                table: "training_program_day");

            migrationBuilder.DropForeignKey(
                name: "FK_workout_exercise_training_program_exercise_TrainingProgramE~",
                table: "workout_exercise");

            migrationBuilder.DropTable(
                name: "training_program_exercise");

            migrationBuilder.DropPrimaryKey(
                name: "PK_exercise_user_progression",
                table: "exercise_user_progression");

            migrationBuilder.DropColumn(
                name: "Repetitions",
                table: "workout_set");

            migrationBuilder.DropColumn(
                name: "WeightKg",
                table: "workout_set");

            migrationBuilder.DropColumn(
                name: "TrainingType",
                table: "workout_exercise");

            migrationBuilder.RenameColumn(
                name: "SetNumber",
                table: "workout_set",
                newName: "Position");

            migrationBuilder.RenameIndex(
                name: "IX_workout_set_WorkoutExerciseId_SetNumber",
                table: "workout_set",
                newName: "IX_workout_set_WorkoutExerciseId_Position");

            migrationBuilder.RenameColumn(
                name: "TrainingProgramExerciseId",
                table: "workout_exercise",
                newName: "TrainingProgramDayExerciseId");

            migrationBuilder.RenameIndex(
                name: "IX_workout_exercise_TrainingProgramExerciseId",
                table: "workout_exercise",
                newName: "IX_workout_exercise_TrainingProgramDayExerciseId");

            migrationBuilder.RenameColumn(
                name: "CompletedAt",
                table: "workout",
                newName: "FinishedAt");

            migrationBuilder.RenameColumn(
                name: "TrainingProgramId",
                table: "training_program_day",
                newName: "TrainingProgramVersionId");

            migrationBuilder.RenameColumn(
                name: "DayNumber",
                table: "training_program_day",
                newName: "Position");

            migrationBuilder.RenameIndex(
                name: "IX_training_program_day_TrainingProgramId_DayNumber",
                table: "training_program_day",
                newName: "IX_training_program_day_TrainingProgramVersionId_Position");

            migrationBuilder.AddColumn<int>(
                name: "ActualRepetitions",
                table: "workout_set",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ActualWeightKg",
                table: "workout_set",
                type: "numeric(7,2)",
                precision: 7,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TargetRepetitions",
                table: "workout_set",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TargetWeightKg",
                table: "workout_set",
                type: "numeric(7,2)",
                precision: 7,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "workout",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "CurrentVersionId",
                table: "training_program",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "exercise_user_progression",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "exercise_user_progression",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddPrimaryKey(
                name: "PK_exercise_user_progression",
                table: "exercise_user_progression",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "training_program_day_exercise",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TrainingProgramDayId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    WeightOverrideKg = table.Column<decimal>(type: "numeric(7,2)", precision: 7, scale: 2, nullable: true),
                    RepetitionsOverride = table.Column<int>(type: "integer", nullable: true),
                    SetsOverride = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_training_program_day_exercise", x => x.Id);
                    table.ForeignKey(
                        name: "FK_training_program_day_exercise_exercise_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "exercise",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_training_program_day_exercise_training_program_day_Training~",
                        column: x => x.TrainingProgramDayId,
                        principalTable: "training_program_day",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "training_program_version",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TrainingProgramId = table.Column<Guid>(type: "uuid", nullable: false),
                    VersionNumber = table.Column<int>(type: "integer", nullable: false),
                    TrainingType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    WorkoutsPerWeek = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_training_program_version", x => x.Id);
                    table.ForeignKey(
                        name: "FK_training_program_version_training_program_TrainingProgramId",
                        column: x => x.TrainingProgramId,
                        principalTable: "training_program",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_training_program_CurrentVersionId",
                table: "training_program",
                column: "CurrentVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_exercise_user_progression_UserId_ExerciseId_TrainingType",
                table: "exercise_user_progression",
                columns: new[] { "UserId", "ExerciseId", "TrainingType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_training_program_day_exercise_ExerciseId",
                table: "training_program_day_exercise",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_training_program_day_exercise_TrainingProgramDayId_Position",
                table: "training_program_day_exercise",
                columns: new[] { "TrainingProgramDayId", "Position" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_training_program_version_TrainingProgramId_VersionNumber",
                table: "training_program_version",
                columns: new[] { "TrainingProgramId", "VersionNumber" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_training_program_training_program_version_CurrentVersionId",
                table: "training_program",
                column: "CurrentVersionId",
                principalTable: "training_program_version",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_training_program_day_training_program_version_TrainingProgr~",
                table: "training_program_day",
                column: "TrainingProgramVersionId",
                principalTable: "training_program_version",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_workout_exercise_training_program_day_exercise_TrainingProg~",
                table: "workout_exercise",
                column: "TrainingProgramDayExerciseId",
                principalTable: "training_program_day_exercise",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_training_program_training_program_version_CurrentVersionId",
                table: "training_program");

            migrationBuilder.DropForeignKey(
                name: "FK_training_program_day_training_program_version_TrainingProgr~",
                table: "training_program_day");

            migrationBuilder.DropForeignKey(
                name: "FK_workout_exercise_training_program_day_exercise_TrainingProg~",
                table: "workout_exercise");

            migrationBuilder.DropTable(
                name: "training_program_day_exercise");

            migrationBuilder.DropTable(
                name: "training_program_version");

            migrationBuilder.DropIndex(
                name: "IX_training_program_CurrentVersionId",
                table: "training_program");

            migrationBuilder.DropPrimaryKey(
                name: "PK_exercise_user_progression",
                table: "exercise_user_progression");

            migrationBuilder.DropIndex(
                name: "IX_exercise_user_progression_UserId_ExerciseId_TrainingType",
                table: "exercise_user_progression");

            migrationBuilder.DropColumn(
                name: "ActualRepetitions",
                table: "workout_set");

            migrationBuilder.DropColumn(
                name: "ActualWeightKg",
                table: "workout_set");

            migrationBuilder.DropColumn(
                name: "TargetRepetitions",
                table: "workout_set");

            migrationBuilder.DropColumn(
                name: "TargetWeightKg",
                table: "workout_set");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "workout");

            migrationBuilder.DropColumn(
                name: "CurrentVersionId",
                table: "training_program");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "exercise_user_progression");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "exercise_user_progression");

            migrationBuilder.RenameColumn(
                name: "Position",
                table: "workout_set",
                newName: "SetNumber");

            migrationBuilder.RenameIndex(
                name: "IX_workout_set_WorkoutExerciseId_Position",
                table: "workout_set",
                newName: "IX_workout_set_WorkoutExerciseId_SetNumber");

            migrationBuilder.RenameColumn(
                name: "TrainingProgramDayExerciseId",
                table: "workout_exercise",
                newName: "TrainingProgramExerciseId");

            migrationBuilder.RenameIndex(
                name: "IX_workout_exercise_TrainingProgramDayExerciseId",
                table: "workout_exercise",
                newName: "IX_workout_exercise_TrainingProgramExerciseId");

            migrationBuilder.RenameColumn(
                name: "FinishedAt",
                table: "workout",
                newName: "CompletedAt");

            migrationBuilder.RenameColumn(
                name: "TrainingProgramVersionId",
                table: "training_program_day",
                newName: "TrainingProgramId");

            migrationBuilder.RenameColumn(
                name: "Position",
                table: "training_program_day",
                newName: "DayNumber");

            migrationBuilder.RenameIndex(
                name: "IX_training_program_day_TrainingProgramVersionId_Position",
                table: "training_program_day",
                newName: "IX_training_program_day_TrainingProgramId_DayNumber");

            migrationBuilder.AddColumn<int>(
                name: "Repetitions",
                table: "workout_set",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "WeightKg",
                table: "workout_set",
                type: "numeric(7,2)",
                precision: 7,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "TrainingType",
                table: "workout_exercise",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_exercise_user_progression",
                table: "exercise_user_progression",
                columns: new[] { "UserId", "ExerciseId", "TrainingType" });

            migrationBuilder.CreateTable(
                name: "training_program_exercise",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExerciseId = table.Column<Guid>(type: "uuid", nullable: false),
                    TrainingProgramDayId = table.Column<Guid>(type: "uuid", nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    TrainingType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_training_program_exercise", x => x.Id);
                    table.ForeignKey(
                        name: "FK_training_program_exercise_exercise_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "exercise",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_training_program_exercise_training_program_day_TrainingProg~",
                        column: x => x.TrainingProgramDayId,
                        principalTable: "training_program_day",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_training_program_exercise_ExerciseId",
                table: "training_program_exercise",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_training_program_exercise_TrainingProgramDayId_Position",
                table: "training_program_exercise",
                columns: new[] { "TrainingProgramDayId", "Position" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_training_program_day_training_program_TrainingProgramId",
                table: "training_program_day",
                column: "TrainingProgramId",
                principalTable: "training_program",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_workout_exercise_training_program_exercise_TrainingProgramE~",
                table: "workout_exercise",
                column: "TrainingProgramExerciseId",
                principalTable: "training_program_exercise",
                principalColumn: "Id");
        }
    }
}
