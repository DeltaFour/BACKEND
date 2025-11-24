using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeltaFour.Infrastructure.Repositories
{
    public class EmployeeShiftRepository(AppDbContext context) : IEmployeeShiftRepository
    {
        public async Task<UserShift?> Find(Expression<Func<UserShift, bool>> predicate)
        {
            return await context.EmployeeShifts.FirstOrDefaultAsync(predicate);
        }
        
        public void Create(UserShift userShift)
        {
            context.EmployeeShifts.Add(userShift);
        }
        
        public void CreateAll(List<UserShift> employeeShifts)
        {
            context.EmployeeShifts.AddRange(employeeShifts);
        }
        
        public void Update(UserShift userShift)
        {
            context.EmployeeShifts.Update(userShift);
        }

        public async Task<Boolean> FindAny(Expression<Func<UserShift, bool>> predicate)
        {
            return await context.EmployeeShifts.AnyAsync(predicate);
        }
        
        public void Delete(UserShift userShift)
        {
            context.EmployeeShifts.Remove(userShift);
        }
        public void UpdateAll(List<UserShift> employeeShifts)
        {
            context.EmployeeShifts.UpdateRange(employeeShifts);
        }
        public void DeleteAll(List<UserShift> employeeShifts)
        {
            context.EmployeeShifts.RemoveRange(employeeShifts);
        }
    }
}