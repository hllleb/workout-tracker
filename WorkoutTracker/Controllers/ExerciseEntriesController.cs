using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Data;
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
        if (workoutId is null)
        {
            return NotFound();
        }

        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        var workout = await GetWorkoutAsync(workoutId.Value, userId);
        if (workout is null)
        {
            return NotFound();
        }

        ViewData["WorkoutName"] = workout.Name;

        var model = new ExerciseEntryEditViewModel
        {
            WorkoutId = workout.Id,
            Sets = 1,
            Reps = 1
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
        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        var workout = await GetWorkoutAsync(model.WorkoutId, userId);
        if (workout is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            ViewData["WorkoutName"] = workout.Name;
            return View(model);
        }

        var entry = new ExerciseEntry
        {
            WorkoutId = workout.Id,
            Name = model.Name,
            Sets = model.Sets,
            Reps = model.Reps,
            WeightKg = model.WeightKg,
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
        if (id is null)
        {
            return NotFound();
        }

        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        var entry = await _context.ExerciseEntries
            .Include(e => e.Workout)
            .FirstOrDefaultAsync(e => e.Id == id && e.Workout != null && e.Workout.UserId == userId);

        if (entry is null || entry.Workout is null)
        {
            return NotFound();
        }

        ViewData["WorkoutName"] = entry.Workout.Name;

        var model = new ExerciseEntryEditViewModel
        {
            Id = entry.Id,
            WorkoutId = entry.WorkoutId,
            Name = entry.Name,
            Sets = entry.Sets,
            Reps = entry.Reps,
            WeightKg = entry.WeightKg,
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
        if (id != model.Id)
        {
            return NotFound();
        }

        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        var entry = await _context.ExerciseEntries
            .Include(e => e.Workout)
            .FirstOrDefaultAsync(e => e.Id == id && e.Workout != null && e.Workout.UserId == userId);

        if (entry is null || entry.Workout is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            ViewData["WorkoutName"] = entry.Workout.Name;
            return View(model);
        }

        entry.Name = model.Name;
        entry.Sets = model.Sets;
        entry.Reps = model.Reps;
        entry.WeightKg = model.WeightKg;
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
        if (id is null)
        {
            return NotFound();
        }

        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        var entry = await _context.ExerciseEntries
            .Include(e => e.Workout)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id && e.Workout != null && e.Workout.UserId == userId);

        if (entry is null || entry.Workout is null)
        {
            return NotFound();
        }

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
        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        var entry = await _context.ExerciseEntries
            .Include(e => e.Workout)
            .FirstOrDefaultAsync(e => e.Id == id && e.Workout != null && e.Workout.UserId == userId);

        if (entry is null || entry.Workout is null)
        {
            return NotFound();
        }

        var workoutId = entry.WorkoutId;

        _context.ExerciseEntries.Remove(entry);
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", "Workouts", new { id = workoutId });
    }

    private string? GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    private Task<Workout?> GetWorkoutAsync(int workoutId, string userId)
    {
        return _context.Workouts
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == workoutId && w.UserId == userId);
    }
}
