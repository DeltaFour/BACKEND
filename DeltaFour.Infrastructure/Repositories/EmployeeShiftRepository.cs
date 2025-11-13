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
        
        public void Create(EmployeeShift employeeShift)
        {
            context.EmployeeShifts.Add(employeeShift);
        }
        
        public void CreateAll(List<EmployeeShift> employeeShifts)
        {
            context.EmployeeShifts.AddRange(employeeShifts);
        }
        
        public void Update(EmployeeShift employeeShift)
        {
            context.EmployeeShifts.Update(employeeShift);
        }

        public async Task<Boolean> FindAny(Expression<Func<EmployeeShift, bool>> predicate)
        {
            return await context.EmployeeShifts.AnyAsync(predicate);
        }
        
        public void Delete(EmployeeShift employeeShift)
        {
            context.EmployeeShifts.Remove(employeeShift);
        }
        public void UpdateAll(List<EmployeeShift> employeeShifts)
        {
            context.EmployeeShifts.UpdateRange(employeeShifts);
        }
        public void DeleteAll(List<EmployeeShift> employeeShifts)
        {
            context.EmployeeShifts.RemoveRange(employeeShifts);
        }
    }
}