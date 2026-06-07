using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeltaFour.Infrastructure.Repositories;

public class TimeSheetAuditRepository(AppDbContext context) : ITimeSheetAuditRepository
{
    public async Task<TimeSheetAudit?> Find(Expression<Func<TimeSheetAudit, bool>> predicate)
    {
        return await context.TimeSheetAudits.FirstOrDefaultAsync(predicate);
    }

    public async Task<List<TimeSheetAudit>> FindByTimeSheet(Guid timeSheetId)
    {
        return await context.TimeSheetAudits
            .Where(a => a.TimeSheetId == timeSheetId)
            .OrderBy(a => a.CreatedAt)
            .ToListAsync();
    }

    public void Create(TimeSheetAudit audit)
    {
        context.TimeSheetAudits.Add(audit);
    }
}
