using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Data;
using WorkoutTracker.Models;

namespace WorkoutTracker.Services.Meals;

/// <summary>
/// EF Core-backed meal queries for the current application user.
/// </summary>
public class MealService : IMealService
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="MealService"/> class.
    /// </summary>
    /// <param name="context">Application database context.</param>
    public MealService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Meal>> GetUserMealsAsync(
        string userId,
        string? search,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Meals
            .Include(meal => meal.Items)
            .Where(meal => meal.UserId == userId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(meal => meal.Name.Contains(search));
        }

        if (fromDate is not null)
        {
            query = query.Where(meal => meal.ConsumedAt >= fromDate.Value.Date);
        }

        if (toDate is not null)
        {
            var endOfDay = toDate.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(meal => meal.ConsumedAt <= endOfDay);
        }

        return await query
            .OrderByDescending(meal => meal.ConsumedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<Meal?> GetUserMealAsync(
        string userId,
        int mealId,
        bool includeItems = false,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Meals.AsQueryable();

        if (includeItems)
        {
            query = query.Include(meal => meal.Items);
        }

        return query.FirstOrDefaultAsync(
            meal => meal.Id == mealId && meal.UserId == userId,
            cancellationToken);
    }
}
