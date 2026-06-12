namespace WorkoutTracker.Helpers;

/// <summary>
/// Determines which cardio exercises support speed tracking (km/h).
/// </summary>
public static class CardioExerciseRules
{
    private static readonly HashSet<string> SpeedCapableNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Running",
        "Jogging",
        "Walking",
        "Cycling"
    };

    /// <summary>
    /// Gets exercise names for which average speed can be recorded.
    /// </summary>
    public static IReadOnlyList<string> SpeedCapableExerciseNames { get; } =
        SpeedCapableNames.OrderBy(n => n, StringComparer.OrdinalIgnoreCase).ToList();

    /// <summary>
    /// Returns whether the given exercise supports speed in km/h.
    /// </summary>
    /// <param name="exerciseName">Exercise name entered by the user.</param>
    public static bool SupportsSpeed(string? exerciseName) =>
        !string.IsNullOrWhiteSpace(exerciseName) && SpeedCapableNames.Contains(exerciseName.Trim());

    /// <summary>
    /// Derives missing duration, distance, or speed when two of the three are provided.
    /// </summary>
    public static void NormalizeSpeedMetrics(
        ref decimal? durationMinutes,
        ref decimal? distanceKm,
        ref decimal? speedKmh)
    {
        if (durationMinutes is > 0 and var duration &&
            distanceKm is > 0 and var distance &&
            speedKmh is null or <= 0)
        {
            speedKmh = Math.Round(distance / (duration / 60m), 2);
            return;
        }

        if (speedKmh is > 0 and var speed && durationMinutes is > 0 and var durationWithSpeed &&
            distanceKm is null or <= 0)
        {
            distanceKm = Math.Round(speed * (durationWithSpeed / 60m), 2);
            return;
        }

        if (speedKmh is > 0 and var speedWithDistance && distanceKm is > 0 and var distanceWithSpeed &&
            durationMinutes is null or <= 0)
        {
            durationMinutes = Math.Round(distanceWithSpeed / speedWithDistance * 60m, 1);
        }
    }
}
