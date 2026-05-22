using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeltaFour.Infrastructure.Repositories;

public class TimeSheetRepository(AppDbContext context) : ITimeSheetRepository
{
    public async Task<TimeSheet?> Find(Expression<Func<TimeSheet, bool>> predicate)
    {
        return await context.TimeSheets
            .Include(t => t.User)
            .FirstOrDefaultAsync(predicate);
    }

    public async Task<List<TimeSheet>> FindAll(Expression<Func<TimeSheet, bool>> predicate)
    {
        return await context.TimeSheets
            .Include(t => t.User)
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<TimeSheet?> FindByUserMonthYear(Guid userId, int month, int year)
    {
        return await context.TimeSheets
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.UserId == userId && t.Month == month && t.Year == year);
    }

    public void Create(TimeSheet timeSheet)
    {
        context.TimeSheets.Add(timeSheet);
    }

    public void Update(TimeSheet timeSheet)
    {
        context.TimeSheets.Update(timeSheet);
    }
}
