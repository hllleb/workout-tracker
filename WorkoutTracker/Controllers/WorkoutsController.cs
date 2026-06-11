using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Data;
using WorkoutTracker.Models;
using WorkoutTracker.ViewModels;

namespace WorkoutTracker.Controllers;

/// <summary>
/// Handles CRUD operations for workouts.
/// </summary>
[Authorize]
public class WorkoutsController : Controller
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkoutsController"/> class.
    /// </summary>
    /// <param name="context">Application database context.</param>
    public WorkoutsController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lists workouts for the current user with optional filters.
    /// </summary>
    /// <param name="search">Optional name search term.</param>
    /// <param name="fromDate">Optional earliest workout date.</param>
    /// <param name="toDate">Optional latest workout date.</param>
    /// <returns>The workouts list view.</returns>
    public async Task<IActionResult> Index(string? search, DateTime? fromDate, DateTime? toDate)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        var query = _context.Workouts.Where(w => w.UserId == userId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(w => w.Name.Contains(search));
        }

        if (fromDate is not null)
        {
            query = query.Where(w => w.WorkoutDate >= fromDate.Value.Date);
        }

        if (toDate is not null)
        {
            query = query.Where(w => w.WorkoutDate <= toDate.Value.Date);
        }

        var workouts = await query
            .OrderByDescending(w => w.WorkoutDate)
            .ToListAsync();

        var model = new WorkoutsIndexViewModel
        {
            Search = search,
            FromDate = fromDate,
            ToDate = toDate,
            Workouts = workouts
        };

        return View(model);
    }

    /// <summary>
    /// Displays workout details.
    /// </summary>
    /// <param name="id">Workout identifier.</param>
    /// <returns>The details view for the workout.</returns>
    public async Task<IActionResult> Details(int? id)
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

        var workout = await _context.Workouts
            .Include(w => w.ExerciseEntries)
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

        if (workout is null)
        {
            return NotFound();
        }

        return View(workout);
    }

    /// <summary>
    /// Displays the workout creation form.
    /// </summary>
    /// <returns>The create view.</returns>
    public IActionResult Create()
    {
        var model = new WorkoutEditViewModel
        {
            WorkoutDate = DateTime.Today
        };

        return View(model);
    }

    /// <summary>
    /// Creates a new workout for the current user.
    /// </summary>
    /// <param name="model">Workout data submitted by the user.</param>
    /// <returns>Redirects to the index view on success.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(WorkoutEditViewModel model)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var workout = new Workout
        {
            Name = model.Name,
            WorkoutDate = model.WorkoutDate,
            Notes = model.Notes,
            UserId = userId
        };

        _context.Workouts.Add(workout);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Displays the edit form for a workout.
    /// </summary>
    /// <param name="id">Workout identifier.</param>
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

        var workout = await _context.Workouts
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

        if (workout is null)
        {
            return NotFound();
        }

        var model = new WorkoutEditViewModel
        {
            Id = workout.Id,
            Name = workout.Name,
            WorkoutDate = workout.WorkoutDate,
            Notes = workout.Notes
        };

        return View(model);
    }

    /// <summary>
    /// Updates an existing workout.
    /// </summary>
    /// <param name="id">Workout identifier.</param>
    /// <param name="model">Workout data submitted by the user.</param>
    /// <returns>Redirects to the index view on success.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, WorkoutEditViewModel model)
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

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var workout = await _context.Workouts
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

        if (workout is null)
        {
            return NotFound();
        }

        workout.Name = model.Name;
        workout.WorkoutDate = model.WorkoutDate;
        workout.Notes = model.Notes;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Displays the delete confirmation view.
    /// </summary>
    /// <param name="id">Workout identifier.</param>
    /// <returns>The delete confirmation view.</returns>
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

        var workout = await _context.Workouts
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

        if (workout is null)
        {
            return NotFound();
        }

        return View(workout);
    }

    /// <summary>
    /// Deletes a workout after confirmation.
    /// </summary>
    /// <param name="id">Workout identifier.</param>
    /// <returns>Redirects to the index view.</returns>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        var workout = await _context.Workouts
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

        if (workout is null)
        {
            return NotFound();
        }

        _context.Workouts.Remove(workout);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private string? GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
