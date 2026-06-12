using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkoutTracker.Migrations
{
    /// <inheritdoc />
    public partial class AddExerciseTypeAndCardioFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CaloriesBurned",
                table: "ExerciseEntries",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DistanceKm",
                table: "ExerciseEntries",
                type: "decimal(6,2)",
                precision: 6,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DurationMinutes",
                table: "ExerciseEntries",
                type: "decimal(6,2)",
                precision: 6,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExerciseType",
                table: "ExerciseEntries",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RepsPerSet",
                table: "ExerciseEntries",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CaloriesBurned",
                table: "ExerciseEntries");

            migrationBuilder.DropColumn(
                name: "DistanceKm",
                table: "ExerciseEntries");

            migrationBuilder.DropColumn(
                name: "DurationMinutes",
                table: "ExerciseEntries");

            migrationBuilder.DropColumn(
                name: "ExerciseType",
                table: "ExerciseEntries");

            migrationBuilder.DropColumn(
                name: "RepsPerSet",
                table: "ExerciseEntries");
        }
    }
}
