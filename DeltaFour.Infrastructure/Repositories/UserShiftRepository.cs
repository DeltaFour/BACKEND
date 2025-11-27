using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeltaFour.Infrastructure.Repositories
{
    public class UserShiftRepository(AppDbContext context) : IUserShiftRepository
    {
        public async Task<UserShift?> Find(Expression<Func<UserShift, bool>> predicate)
        {
            return await context.EmployeeShifts.FirstOrDefaultAsync(predicate);
        }
        
        public void Create(UserShift userShift)
        {
            context.EmployeeShifts.Add(userShift);
        }
        
        public void CreateAll(List<UserShift> userShifts)
        {
            context.EmployeeShifts.AddRange(userShifts);
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
        public void UpdateAll(List<UserShift> userShifts)
        {
            context.EmployeeShifts.UpdateRange(userShifts);
        }
        public void DeleteAll(List<UserShift> userShifts)
        {
            context.EmployeeShifts.RemoveRange(userShifts);
        }
    }
}