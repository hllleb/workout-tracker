using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Data;
using WorkoutTracker.Models;
using WorkoutTracker.ViewModels;

namespace WorkoutTracker.Controllers;

/// <summary>
/// Handles CRUD operations for meal items.
/// </summary>
[Authorize]
public class MealItemsController : Controller
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="MealItemsController"/> class.
    /// </summary>
    /// <param name="context">Application database context.</param>
    public MealItemsController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Displays the meal item creation form.
    /// </summary>
    /// <param name="mealId">Meal identifier.</param>
    /// <returns>The create view.</returns>
    public async Task<IActionResult> Create(int? mealId)
    {
        if (mealId is null)
        {
            return NotFound();
        }

        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        var meal = await GetMealAsync(mealId.Value, userId);
        if (meal is null)
        {
            return NotFound();
        }

        ViewData["MealName"] = meal.Name;

        var model = new MealItemEditViewModel
        {
            MealId = meal.Id,
            Quantity = 1
        };

        return View(model);
    }

    /// <summary>
    /// Creates a new meal item.
    /// </summary>
    /// <param name="model">Item data submitted by the user.</param>
    /// <returns>Redirects to the meal details view on success.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MealItemEditViewModel model)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        var meal = await GetMealAsync(model.MealId, userId);
        if (meal is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            ViewData["MealName"] = meal.Name;
            return View(model);
        }

        var item = new MealItem
        {
            MealId = meal.Id,
            Name = model.Name,
            Quantity = model.Quantity,
            Unit = model.Unit,
            Calories = model.Calories,
            ProteinG = model.ProteinG,
            CarbsG = model.CarbsG,
            FatG = model.FatG
        };

        _context.MealItems.Add(item);
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", "Meals", new { id = meal.Id });
    }

    /// <summary>
    /// Displays the edit form for a meal item.
    /// </summary>
    /// <param name="id">Item identifier.</param>
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

        var item = await _context.MealItems
            .Include(i => i.Meal)
            .FirstOrDefaultAsync(i => i.Id == id && i.Meal != null && i.Meal.UserId == userId);

        if (item is null || item.Meal is null)
        {
            return NotFound();
        }

        ViewData["MealName"] = item.Meal.Name;

        var model = new MealItemEditViewModel
        {
            Id = item.Id,
            MealId = item.MealId,
            Name = item.Name,
            Quantity = item.Quantity,
            Unit = item.Unit,
            Calories = item.Calories,
            ProteinG = item.ProteinG,
            CarbsG = item.CarbsG,
            FatG = item.FatG
        };

        return View(model);
    }

    /// <summary>
    /// Updates an existing meal item.
    /// </summary>
    /// <param name="id">Item identifier.</param>
    /// <param name="model">Item data submitted by the user.</param>
    /// <returns>Redirects to the meal details view on success.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, MealItemEditViewModel model)
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

        var item = await _context.MealItems
            .Include(i => i.Meal)
            .FirstOrDefaultAsync(i => i.Id == id && i.Meal != null && i.Meal.UserId == userId);

        if (item is null || item.Meal is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            ViewData["MealName"] = item.Meal.Name;
            return View(model);
        }

        item.Name = model.Name;
        item.Quantity = model.Quantity;
        item.Unit = model.Unit;
        item.Calories = model.Calories;
        item.ProteinG = model.ProteinG;
        item.CarbsG = model.CarbsG;
        item.FatG = model.FatG;

        await _context.SaveChangesAsync();

        return RedirectToAction("Details", "Meals", new { id = item.MealId });
    }

    /// <summary>
    /// Displays the delete confirmation view.
    /// </summary>
    /// <param name="id">Item identifier.</param>
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

        var item = await _context.MealItems
            .Include(i => i.Meal)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id && i.Meal != null && i.Meal.UserId == userId);

        if (item is null || item.Meal is null)
        {
            return NotFound();
        }

        ViewData["MealName"] = item.Meal.Name;
        return View(item);
    }

    /// <summary>
    /// Deletes a meal item after confirmation.
    /// </summary>
    /// <param name="id">Item identifier.</param>
    /// <returns>Redirects to the meal details view.</returns>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            return Challenge();
        }

        var item = await _context.MealItems
            .Include(i => i.Meal)
            .FirstOrDefaultAsync(i => i.Id == id && i.Meal != null && i.Meal.UserId == userId);

        if (item is null || item.Meal is null)
        {
            return NotFound();
        }

        var mealId = item.MealId;

        _context.MealItems.Remove(item);
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", "Meals", new { id = mealId });
    }

    private string? GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    private Task<Meal?> GetMealAsync(int mealId, string userId)
    {
        return _context.Meals
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == mealId && m.UserId == userId);
    }
}
