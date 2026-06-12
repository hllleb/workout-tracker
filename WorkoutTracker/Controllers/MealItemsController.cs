using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Data;
using WorkoutTracker.Models;
using WorkoutTracker.Services.FatSecret;
using WorkoutTracker.Services.Meals;
using WorkoutTracker.Services.Products;
using WorkoutTracker.ViewModels;

namespace WorkoutTracker.Controllers;

/// <summary>
/// Handles CRUD operations for meal items.
/// </summary>
[Authorize]
public class MealItemsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IFatSecretClient _fatSecret;
    private readonly IMealService _mealService;
    private readonly IProductCacheService _productCacheService;

    /// <summary>
    /// Initializes a new instance of the <see cref="MealItemsController"/> class.
    /// </summary>
    /// <param name="context">Application database context.</param>
    /// <param name="fatSecret">FatSecret API client.</param>
    /// <param name="mealService">Meal query service.</param>
    /// <param name="productCacheService">Product cache service.</param>
    public MealItemsController(
        ApplicationDbContext context,
        IFatSecretClient fatSecret,
        IMealService mealService,
        IProductCacheService productCacheService)
    {
        _context = context;
        _fatSecret = fatSecret;
        _mealService = mealService;
        _productCacheService = productCacheService;
    }

    /// <summary>
    /// Displays a prefilled meal item form based on FatSecret data.
    /// </summary>
    /// <param name="mealId">Meal identifier.</param>
    /// <param name="foodId">FatSecret food identifier.</param>
    /// <param name="servingId">FatSecret serving identifier.</param>
    /// <returns>The create view with prefilled data.</returns>
    public async Task<IActionResult> CreateFromFatSecret(int? mealId, string? foodId, string? servingId)
    {
        if (mealId is null || string.IsNullOrWhiteSpace(foodId))
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

        FatSecretFoodDetails? food;
        try
        {
            food = await _fatSecret.GetFoodAsync(foodId, HttpContext.RequestAborted);
        }
        catch
        {
            var errorModel = new MealItemEditViewModel { MealId = meal.Id };
            return View("FatSecretError", errorModel);
        }

        if (food is null)
        {
            var notFoundModel = new MealItemEditViewModel { MealId = meal.Id };
            return View("CreateFromFatSecretNotFound", notFoundModel);
        }

        var serving = food.Servings.FirstOrDefault(s => s.ServingId == servingId)
            ?? food.Servings.FirstOrDefault();

        if (serving is null)
        {
            var notFoundModel = new MealItemEditViewModel { MealId = meal.Id };
            return View("CreateFromFatSecretNotFound", notFoundModel);
        }

        var product = await _productCacheService.GetOrCreateFromFatSecretAsync(
            food,
            serving,
            HttpContext.RequestAborted);

        // Quantity = number of servings (default 1); Unit = serving description (e.g. "1 slice (28g)")
        // CaloriesPerUnit lets JS recalculate when user changes quantity.
        var model = new MealItemEditViewModel
        {
            MealId = meal.Id,
            ProductId = product.Id,
            Name = food.Name,
            Quantity = 1,
            Unit = serving.Description ?? serving.MetricServingUnit,
            Calories = serving.Calories ?? 0,
            CaloriesPerUnit = serving.Calories ?? 0,
            ProteinG = serving.ProteinG,
            ProteinPerUnit = serving.ProteinG,
            CarbsG = serving.CarbsG,
            CarbsPerUnit = serving.CarbsG,
            FatG = serving.FatG,
            FatPerUnit = serving.FatG
        };

        return View("Create", model);
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
            await PopulatePerUnitFromProductAsync(model);
            return View(model);
        }

        await ApplyProductNutritionAsync(model);

        var item = new MealItem
        {
            MealId = meal.Id,
            ProductId = model.ProductId,
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
            .Include(i => i.Product)
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
            ProductId = item.ProductId,
            Name = item.Name,
            Quantity = item.Quantity,
            Unit = item.Unit,
            Calories = item.Calories,
            ProteinG = item.ProteinG,
            CarbsG = item.CarbsG,
            FatG = item.FatG
        };

        PopulatePerUnitFromProduct(model, item.Product);

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
            await PopulatePerUnitFromProductAsync(model);
            return View(model);
        }

        await ApplyProductNutritionAsync(model);

        item.ProductId = model.ProductId;
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
        return _mealService.GetUserMealAsync(userId, mealId, cancellationToken: HttpContext.RequestAborted);
    }

    private static void PopulatePerUnitFromProduct(MealItemEditViewModel model, Product? product)
    {
        if (model.ProductId is null || product is null)
        {
            return;
        }

        model.Name = product.Name;
        model.CaloriesPerUnit = product.Calories;
        model.ProteinPerUnit = product.ProteinG;
        model.CarbsPerUnit = product.CarbsG;
        model.FatPerUnit = product.FatG;
    }

    private async Task PopulatePerUnitFromProductAsync(MealItemEditViewModel model)
    {
        if (model.ProductId is null)
        {
            return;
        }

        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == model.ProductId);

        PopulatePerUnitFromProduct(model, product);
    }

    private async Task ApplyProductNutritionAsync(MealItemEditViewModel model)
    {
        if (model.ProductId is null)
        {
            return;
        }

        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == model.ProductId);

        if (product is null)
        {
            return;
        }

        model.Name = product.Name;

        var quantity = model.Quantity;
        model.Calories = (int)Math.Round(product.Calories * quantity, MidpointRounding.AwayFromZero);
        model.ProteinG = product.ProteinG.HasValue
            ? Math.Round(product.ProteinG.Value * quantity, 2, MidpointRounding.AwayFromZero)
            : null;
        model.CarbsG = product.CarbsG.HasValue
            ? Math.Round(product.CarbsG.Value * quantity, 2, MidpointRounding.AwayFromZero)
            : null;
        model.FatG = product.FatG.HasValue
            ? Math.Round(product.FatG.Value * quantity, 2, MidpointRounding.AwayFromZero)
            : null;
    }
}
