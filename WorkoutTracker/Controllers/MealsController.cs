using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Data;
using WorkoutTracker.Models;
using WorkoutTracker.ViewModels;

namespace WorkoutTracker.Controllers;

/// <summary>
/// Handles CRUD operations for meals.
/// </summary>
[Authorize]
public class MealsController : Controller
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="MealsController"/> class.
    /// </summary>
    /// <param name="context">Application database context.</param>
    public MealsController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lists meals for the current user.
    /// </summary>
    /// <returns>The meals list view.</returns>
    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        var meals = await _context.Meals
            .Where(m => m.UserId == userId)
            .OrderByDescending(m => m.ConsumedAt)
            .ToListAsync();

        return View(meals);
    }

    /// <summary>
    /// Displays meal details.
    /// </summary>
    /// <param name="id">Meal identifier.</param>
    /// <returns>The details view for the meal.</returns>
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

        var meal = await _context.Meals
            .Include(m => m.Items)
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

        if (meal is null)
        {
            return NotFound();
        }

        return View(meal);
    }

    /// <summary>
    /// Displays the meal creation form.
    /// </summary>
    /// <returns>The create view.</returns>
    public IActionResult Create()
    {
        var model = new MealEditViewModel
        {
            ConsumedAt = DateTime.Now
        };

        return View(model);
    }

    /// <summary>
    /// Creates a new meal for the current user.
    /// </summary>
    /// <param name="model">Meal data submitted by the user.</param>
    /// <returns>Redirects to the index view on success.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MealEditViewModel model)
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

        var meal = new Meal
        {
            Name = model.Name,
            ConsumedAt = model.ConsumedAt,
            Notes = model.Notes,
            UserId = userId
        };

        _context.Meals.Add(meal);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Displays the edit form for a meal.
    /// </summary>
    /// <param name="id">Meal identifier.</param>
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

        var meal = await _context.Meals
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

        if (meal is null)
        {
            return NotFound();
        }

        var model = new MealEditViewModel
        {
            Id = meal.Id,
            Name = meal.Name,
            ConsumedAt = meal.ConsumedAt,
            Notes = meal.Notes
        };

        return View(model);
    }

    /// <summary>
    /// Updates an existing meal.
    /// </summary>
    /// <param name="id">Meal identifier.</param>
    /// <param name="model">Meal data submitted by the user.</param>
    /// <returns>Redirects to the index view on success.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, MealEditViewModel model)
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

        var meal = await _context.Meals
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

        if (meal is null)
        {
            return NotFound();
        }

        meal.Name = model.Name;
        meal.ConsumedAt = model.ConsumedAt;
        meal.Notes = model.Notes;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Displays the delete confirmation view.
    /// </summary>
    /// <param name="id">Meal identifier.</param>
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

        var meal = await _context.Meals
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

        if (meal is null)
        {
            return NotFound();
        }

        return View(meal);
    }

    /// <summary>
    /// Deletes a meal after confirmation.
    /// </summary>
    /// <param name="id">Meal identifier.</param>
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

        var meal = await _context.Meals
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

        if (meal is null)
        {
            return NotFound();
        }

        _context.Meals.Remove(meal);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private string? GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
