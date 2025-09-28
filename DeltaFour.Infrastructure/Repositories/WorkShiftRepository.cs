using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeltaFour.Infrastructure.Repositories
{
    public class WorkShiftRepository(AppDbContext context) : IWorkShiftRepository
    {
        public async Task<WorkShift?> Find(Expression<Func<WorkShift, bool>> predicate)
        {
            return await context.WorkShifts.FirstOrDefaultAsync(predicate);
        }
        public async Task Create(WorkShift workShift)
        {
            context.WorkShifts.Add(workShift);
            await context.SaveChangesAsync();
        }
        public async Task Update(WorkShift workShift)
        {
            context.WorkShifts.Update(workShift);
            await context.SaveChangesAsync();
        }
        public async Task Delete(WorkShift workShift)
        {
            context.WorkShifts.Remove(workShift);
            await context.SaveChangesAsync();
        }
    }
}