using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeltaFour.Infrastructure.Repositories
{
    public class EmployeeAttendanceRepository(AppDbContext context) : IEmployeeAttendanceRepository
    {
        public async Task<EmployeeAttendance?> Find(Expression<Func<EmployeeAttendance, bool>> predicate)
        {
            return await context.EmployeeAttendances.FirstOrDefaultAsync(predicate);
        }
        public async Task Create(EmployeeAttendance employeeAttendance)
        {
            context.EmployeeAttendances.Add(employeeAttendance);
            await context.SaveChangesAsync();
        }
        public async Task Update(EmployeeAttendance employeeAttendance)
        {
            context.EmployeeAttendances.Update(employeeAttendance);
            await context.SaveChangesAsync();
        }
        public async Task Delete(EmployeeAttendance employeeAttendance)
        {
            context.EmployeeAttendances.Remove(employeeAttendance);
            await context.SaveChangesAsync();
        }
    }
}