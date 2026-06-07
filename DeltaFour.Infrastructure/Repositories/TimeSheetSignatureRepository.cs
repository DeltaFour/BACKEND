using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeltaFour.Infrastructure.Repositories;

public class TimeSheetSignatureRepository(AppDbContext context) : ITimeSheetSignatureRepository
{
    public async Task<TimeSheetSignature?> Find(Expression<Func<TimeSheetSignature, bool>> predicate)
    {
        return await context.TimeSheetSignatures.FirstOrDefaultAsync(predicate);
    }

    public async Task<List<TimeSheetSignature>> FindByTimeSheet(Guid timeSheetId)
    {
        return await context.TimeSheetSignatures
            .Where(s => s.TimeSheetId == timeSheetId)
            .OrderBy(s => s.SignedAtUtc)
            .ToListAsync();
    }

    public void Create(TimeSheetSignature signature)
    {
        context.TimeSheetSignatures.Add(signature);
    }
}
