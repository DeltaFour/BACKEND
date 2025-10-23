using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Domain.ValueObjects.Dtos;
using DeltaFour.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeltaFour.Infrastructure.Repositories
{
    public class EmployeeRepository(AppDbContext context) : IEmployeeRepository
    {
        public async Task<List<EmployeeResponseDto>> GetAll(Guid companyId)
        {
            return await context.Employees.Where(e => e.IsActive == true && e.CompanyId == companyId)
                .Select(e => new EmployeeResponseDto()
                {
                    Id = e.Id,
                    Name = e.Name,
                    Cellphone = e.Cellphone,
                    Email = e.Email,
                    RoleName = e.Role!.Name,
                    IsActive = e.IsActive,
                    IsAllowedBypassCoord = e.IsAllowedBypassCoord,
                    LastLogin = e.LastLogin,
                    ShiftDto = e.EmployeeShifts!.Select(s => new EmployeeResponseShiftsDto()
                    {
                        Id = s.Id,
                        StartDate = s.StartDate,
                        EndDate = s.EndDate,
                        WorkShiftType = s.WorkShift!.ShiftType,
                        WorkShiftStartTime = s.WorkShift.StartTime,
                        WorkShiftEndTime = s.WorkShift.EndTime,
                        WorkShiftToleranceMinutes = s.WorkShift.ToleranceMinutes
                    }).ToList()
                }).ToListAsync();
        }

        public async Task<bool> FindAny(Expression<Func<Employee, bool>> predicate)
        {
            return await context.Employees.AnyAsync(predicate);
        }
        public async Task<Employee?> FindIncluding(Guid id)
        {
            return await context.Employees.Where(e => e.Id == id)
                .Include(e => e.EmployeeShifts).SingleOrDefaultAsync();
        }

        public void Create(Employee employee)
        {
            context.Employees.Add(employee);
        }

        public void Update(Employee employee)
        {
            context.Employees.Update(employee);
        }

        public async Task<Employee?> Find(Expression<Func<Employee, bool>> predicate)
        {
            return await context.Employees.FirstOrDefaultAsync(predicate);
        }
    }
}
