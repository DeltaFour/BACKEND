using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeltaFour.Infrastructure.Repositories;

public class TimeSheetSignatureTokenRepository(AppDbContext context) : ITimeSheetSignatureTokenRepository
{
    public async Task<TimeSheetSignatureToken?> Find(Expression<Func<TimeSheetSignatureToken, bool>> predicate)
    {
        return await context.TimeSheetSignatureTokens.FirstOrDefaultAsync(predicate);
    }

    public async Task<TimeSheetSignatureToken?> FindByToken(string token)
    {
        return await context.TimeSheetSignatureTokens
            .FirstOrDefaultAsync(t => t.Token == token);
    }

    public async Task<List<TimeSheetSignatureToken>> FindByTimeSheet(Guid timeSheetId)
    {
        return await context.TimeSheetSignatureTokens
            .Where(t => t.TimeSheetId == timeSheetId)
            .OrderBy(t => t.CreatedAt)
            .ToListAsync();
    }

    public void Create(TimeSheetSignatureToken token)
    {
        context.TimeSheetSignatureTokens.Add(token);
    }

    public void Update(TimeSheetSignatureToken token)
    {
        context.TimeSheetSignatureTokens.Update(token);
    }
}
