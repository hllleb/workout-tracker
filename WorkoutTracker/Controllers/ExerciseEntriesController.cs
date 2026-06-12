using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Data;
using WorkoutTracker.Helpers;
using WorkoutTracker.Models;
using WorkoutTracker.ViewModels;

namespace WorkoutTracker.Controllers;

/// <summary>
/// Handles CRUD operations for exercise entries.
/// </summary>
[Authorize]
public class ExerciseEntriesController : Controller
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExerciseEntriesController"/> class.
    /// </summary>
    /// <param name="context">Application database context.</param>
    public ExerciseEntriesController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Displays the exercise creation form.
    /// </summary>
    /// <param name="workoutId">Workout identifier.</param>
    /// <returns>The create view.</returns>
    public async Task<IActionResult> Create(int? workoutId)
    {
        if (workoutId is null) return NotFound();

        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return Challenge();

        var workout = await GetWorkoutAsync(workoutId.Value, userId);
        if (workout is null) return NotFound();

        ViewData["WorkoutName"] = workout.Name;

        var model = new ExerciseEntryEditViewModel
        {
            WorkoutId = workout.Id,
            Sets = 3,
            RepsPerSetValues = Enumerable.Repeat(10, 3).ToList()
        };

        return View(model);
    }

    /// <summary>
    /// Creates a new exercise entry.
    /// </summary>
    /// <param name="model">Exercise data submitted by the user.</param>
    /// <returns>Redirects to the workout details view on success.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ExerciseEntryEditViewModel model)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return Challenge();

        var workout = await GetWorkoutAsync(model.WorkoutId, userId);
        if (workout is null) return NotFound();

        if (!ModelState.IsValid)
        {
            ViewData["WorkoutName"] = workout.Name;
            return View(model);
        }

        ApplyCardioMetrics(model);

        var (repsAvg, repsJson) = ProcessReps(model);

        var entry = new ExerciseEntry
        {
            WorkoutId = workout.Id,
            Name = model.Name,
            ExerciseType = model.ExerciseType,
            Sets = model.ExerciseType == "Cardio" ? 1 : model.Sets,
            Reps = repsAvg,
            RepsPerSet = repsJson,
            WeightKg = model.ExerciseType == "Cardio" ? null : model.WeightKg,
            DurationMinutes = model.ExerciseType == "Cardio" ? model.DurationMinutes : null,
            DistanceKm = model.ExerciseType == "Cardio" ? model.DistanceKm : null,
            SpeedKmh = model.ExerciseType == "Cardio" && CardioExerciseRules.SupportsSpeed(model.Name)
                ? model.SpeedKmh
                : null,
            CaloriesBurned = EstimateCalories(model),
            Notes = model.Notes
        };

        _context.ExerciseEntries.Add(entry);
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", "Workouts", new { id = workout.Id });
    }

    /// <summary>
    /// Displays the edit form for an exercise entry.
    /// </summary>
    /// <param name="id">Entry identifier.</param>
    /// <returns>The edit view.</returns>
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound();

        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return Challenge();

        var entry = await _context.ExerciseEntries
            .Include(e => e.Workout)
            .FirstOrDefaultAsync(e => e.Id == id && e.Workout != null && e.Workout.UserId == userId);

        if (entry is null || entry.Workout is null) return NotFound();

        ViewData["WorkoutName"] = entry.Workout.Name;

        var repsPerSetValues = ParseRepsPerSet(entry.RepsPerSet, entry.Sets, entry.Reps);

        var model = new ExerciseEntryEditViewModel
        {
            Id = entry.Id,
            WorkoutId = entry.WorkoutId,
            Name = entry.Name,
            ExerciseType = entry.ExerciseType,
            Sets = entry.Sets,
            RepsPerSetValues = repsPerSetValues,
            WeightKg = entry.WeightKg,
            DurationMinutes = entry.DurationMinutes,
            DistanceKm = entry.DistanceKm,
            SpeedKmh = entry.SpeedKmh,
            CaloriesBurned = entry.CaloriesBurned,
            Notes = entry.Notes
        };

        return View(model);
    }

    /// <summary>
    /// Updates an existing exercise entry.
    /// </summary>
    /// <param name="id">Entry identifier.</param>
    /// <param name="model">Exercise data submitted by the user.</param>
    /// <returns>Redirects to the workout details view on success.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ExerciseEntryEditViewModel model)
    {
        if (id != model.Id) return NotFound();

        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return Challenge();

        var entry = await _context.ExerciseEntries
            .Include(e => e.Workout)
            .FirstOrDefaultAsync(e => e.Id == id && e.Workout != null && e.Workout.UserId == userId);

        if (entry is null || entry.Workout is null) return NotFound();

        if (!ModelState.IsValid)
        {
            ViewData["WorkoutName"] = entry.Workout.Name;
            return View(model);
        }

        ApplyCardioMetrics(model);

        var (repsAvg, repsJson) = ProcessReps(model);

        entry.Name = model.Name;
        entry.ExerciseType = model.ExerciseType;
        entry.Sets = model.ExerciseType == "Cardio" ? 1 : model.Sets;
        entry.Reps = repsAvg;
        entry.RepsPerSet = repsJson;
        entry.WeightKg = model.ExerciseType == "Cardio" ? null : model.WeightKg;
        entry.DurationMinutes = model.ExerciseType == "Cardio" ? model.DurationMinutes : null;
        entry.DistanceKm = model.ExerciseType == "Cardio" ? model.DistanceKm : null;
        entry.SpeedKmh = model.ExerciseType == "Cardio" && CardioExerciseRules.SupportsSpeed(model.Name)
            ? model.SpeedKmh
            : null;
        entry.CaloriesBurned = EstimateCalories(model);
        entry.Notes = model.Notes;

        await _context.SaveChangesAsync();

        return RedirectToAction("Details", "Workouts", new { id = entry.WorkoutId });
    }

    /// <summary>
    /// Displays the delete confirmation view.
    /// </summary>
    /// <param name="id">Entry identifier.</param>
    /// <returns>The delete view.</returns>
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null) return NotFound();

        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return Challenge();

        var entry = await _context.ExerciseEntries
            .Include(e => e.Workout)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id && e.Workout != null && e.Workout.UserId == userId);

        if (entry is null || entry.Workout is null) return NotFound();

        ViewData["WorkoutName"] = entry.Workout.Name;
        return View(entry);
    }

    /// <summary>
    /// Deletes an exercise entry after confirmation.
    /// </summary>
    /// <param name="id">Entry identifier.</param>
    /// <returns>Redirects to the workout details view.</returns>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId)) return Challenge();

        var entry = await _context.ExerciseEntries
            .Include(e => e.Workout)
            .FirstOrDefaultAsync(e => e.Id == id && e.Workout != null && e.Workout.UserId == userId);

        if (entry is null || entry.Workout is null) return NotFound();

        var workoutId = entry.WorkoutId;
        _context.ExerciseEntries.Remove(entry);
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", "Workouts", new { id = workoutId });
    }

    // ─── helpers ────────────────────────────────────────────────────────────────

    private string? GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);

    private Task<Workout?> GetWorkoutAsync(int workoutId, string userId) =>
        _context.Workouts
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == workoutId && w.UserId == userId);

    private static (int avg, string? json) ProcessReps(ExerciseEntryEditViewModel model)
    {
        if (model.ExerciseType == "Cardio")
            return (0, null);

        var values = model.RepsPerSetValues;
        if (values is null || values.Count == 0)
            return (1, null);

        var trimmed = values.Take(model.Sets).ToList();
        if (trimmed.Count == 0)
            return (1, null);

        var avg = (int)Math.Round(trimmed.Average());
        var json = JsonSerializer.Serialize(trimmed);
        return (avg, json);
    }

    private static List<int> ParseRepsPerSet(string? json, int sets, int fallbackReps)
    {
        if (!string.IsNullOrWhiteSpace(json))
        {
            try
            {
                var list = JsonSerializer.Deserialize<List<int>>(json);
                if (list is { Count: > 0 })
                    return list;
            }
            catch { /* ignore malformed JSON */ }
        }

        return Enumerable.Repeat(fallbackReps > 0 ? fallbackReps : 1, Math.Max(sets, 1)).ToList();
    }

    private static void ApplyCardioMetrics(ExerciseEntryEditViewModel model)
    {
        if (model.ExerciseType != "Cardio")
        {
            model.DurationMinutes = null;
            model.DistanceKm = null;
            model.SpeedKmh = null;
            return;
        }

        if (!CardioExerciseRules.SupportsSpeed(model.Name))
        {
            model.SpeedKmh = null;
            return;
        }

        var duration = model.DurationMinutes;
        var distance = model.DistanceKm;
        var speed = model.SpeedKmh;
        CardioExerciseRules.NormalizeSpeedMetrics(ref duration, ref distance, ref speed);
        model.DurationMinutes = duration;
        model.DistanceKm = distance;
        model.SpeedKmh = speed;
    }

    private static int? EstimateCalories(ExerciseEntryEditViewModel model)
    {
        if (model.ExerciseType == "Cardio")
        {
            if (model.DurationMinutes is null or <= 0) return null;
            // MET-based estimate for running/cycling (MET ≈ 8), assumed 75 kg user
            var met = model.DistanceKm > 0 ? 8.5 : 6.0; // higher MET if distance recorded
            return (int)Math.Round(met * 75 * (double)model.DurationMinutes.Value / 60);
        }

        // Strength: estimate based on sets × avg reps × weight
        var sets = model.Sets;
        var reps = model.RepsPerSetValues is { Count: > 0 }
            ? (double)model.RepsPerSetValues.Average()
            : 10.0;
        var weightKg = (double)(model.WeightKg ?? 20m);
        // ~0.35 cal per rep + 0.015 cal per kg per rep
        var cal = sets * reps * (0.35 + weightKg * 0.015);
        return cal > 0 ? (int)Math.Round(cal) : null;
    }
}
