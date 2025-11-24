using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeltaFour.Infrastructure.Repositories
{
    public class EmployeeAttendanceRepository(AppDbContext context) : IEmployeeAttendanceRepository
    {
        public async Task<UserAttendance?> Find(Expression<Func<UserAttendance, bool>> predicate)
        {
            return await context.EmployeeAttendances.FirstOrDefaultAsync(predicate);
        }
        public void Create(UserAttendance userAttendance)
        {
            context.EmployeeAttendances.Add(userAttendance);
        }
        public void Update(UserAttendance userAttendance)
        {
            context.EmployeeAttendances.Update(userAttendance);
        }
        public void Delete(UserAttendance userAttendance)
        {
            context.EmployeeAttendances.Remove(userAttendance);
        }
    }
}