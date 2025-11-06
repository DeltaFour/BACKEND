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
        public void Create(WorkShift workShift)
        {
            context.WorkShifts.Add(workShift);
        }
        public void Update(WorkShift workShift)
        {
            context.WorkShifts.Update(workShift);
        }
        public void Delete(WorkShift workShift)
        {
            context.WorkShifts.Remove(workShift);
        }
    }
}