using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Data;
using WorkoutTracker.Models;
using WorkoutTracker.ViewModels;

namespace WorkoutTracker.Controllers;

/// <summary>
/// Handles admin management of cached products.
/// </summary>
[Authorize(Roles = "Admin")]
public class ProductsController : Controller
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductsController"/> class.
    /// </summary>
    /// <param name="context">Application database context.</param>
    public ProductsController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lists cached products with optional filters.
    /// </summary>
    /// <param name="search">Optional name search term.</param>
    /// <param name="source">Optional source filter.</param>
    /// <returns>The products list view.</returns>
    public async Task<IActionResult> Index(string? search, string? source)
    {
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.Name.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(source))
        {
            query = query.Where(p => p.Source == source);
        }

        var products = await query
            .OrderByDescending(p => p.CachedAt)
            .Select(p => new ProductListItemViewModel
            {
                Id = p.Id,
                ExternalId = p.ExternalId,
                Source = p.Source,
                Name = p.Name,
                Calories = p.Calories,
                CachedAt = p.CachedAt,
                MealItemCount = p.MealItems.Count
            })
            .ToListAsync();

        var availableSources = await _context.Products
            .Select(p => p.Source)
            .Distinct()
            .OrderBy(s => s)
            .ToListAsync();

        var model = new ProductsIndexViewModel
        {
            Search = search,
            Source = source,
            Products = products,
            AvailableSources = availableSources
        };

        return View(model);
    }

    /// <summary>
    /// Displays product details.
    /// </summary>
    /// <param name="id">Product identifier.</param>
    /// <returns>The details view.</returns>
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var product = await _context.Products
            .AsNoTracking()
            .Include(p => p.MealItems)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
        {
            return NotFound();
        }

        return View(product);
    }

    /// <summary>
    /// Displays the edit form for a product.
    /// </summary>
    /// <param name="id">Product identifier.</param>
    /// <returns>The edit view.</returns>
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
        {
            return NotFound();
        }

        return View(MapToEditViewModel(product));
    }

    /// <summary>
    /// Updates a cached product.
    /// </summary>
    /// <param name="id">Product identifier.</param>
    /// <param name="model">Product data submitted by the admin.</param>
    /// <returns>Redirects to the index view on success.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductEditViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product is null)
        {
            return NotFound();
        }

        product.Name = model.Name;
        product.ServingSize = model.ServingSize;
        product.ServingUnit = model.ServingUnit;
        product.Calories = model.Calories;
        product.ProteinG = model.ProteinG;
        product.CarbsG = model.CarbsG;
        product.FatG = model.FatG;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Displays the delete confirmation view.
    /// </summary>
    /// <param name="id">Product identifier.</param>
    /// <returns>The delete confirmation view.</returns>
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var product = await _context.Products
            .AsNoTracking()
            .Include(p => p.MealItems)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
        {
            return NotFound();
        }

        return View(product);
    }

    /// <summary>
    /// Deletes a cached product after confirmation.
    /// </summary>
    /// <param name="id">Product identifier.</param>
    /// <returns>Redirects to the index view.</returns>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await _context.Products
            .Include(p => p.MealItems)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
        {
            return NotFound();
        }

        foreach (var mealItem in product.MealItems)
        {
            mealItem.ProductId = null;
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private static ProductEditViewModel MapToEditViewModel(Product product)
    {
        return new ProductEditViewModel
        {
            Id = product.Id,
            ExternalId = product.ExternalId,
            Source = product.Source,
            Name = product.Name,
            ServingSize = product.ServingSize,
            ServingUnit = product.ServingUnit,
            Calories = product.Calories,
            ProteinG = product.ProteinG,
            CarbsG = product.CarbsG,
            FatG = product.FatG,
            CachedAt = product.CachedAt
        };
    }
}
