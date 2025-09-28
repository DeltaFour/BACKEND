using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeltaFour.Infrastructure.Repositories
{
    public class EmployeeShiftRepository(AppDbContext context) : IEmployeeShiftRepository
    {
        public async Task<EmployeeShift?> Find(Expression<Func<EmployeeShift, bool>> predicate)
        {
            return await context.EmployeeShifts.FirstOrDefaultAsync(predicate);
        }
        public async Task Create(EmployeeShift employeeShift)
        {
            context.EmployeeShifts.Add(employeeShift);
            await context.SaveChangesAsync();
        }
        public async Task Update(EmployeeShift employeeShift)
        {
            context.EmployeeShifts.Update(employeeShift);
            await context.SaveChangesAsync();
        }
        public async Task Delete(EmployeeShift employeeShift)
        {
            context.EmployeeShifts.Remove(employeeShift);
            await context.SaveChangesAsync();
        }
    }
}