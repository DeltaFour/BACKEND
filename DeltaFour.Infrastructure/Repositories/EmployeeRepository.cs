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
                        IsActive = s.IsActive,
                        WorkShiftType = s.WorkShift!.ShiftType,
                        WorkShiftStartTime = s.WorkShift.StartTime,
                        WorkShiftEndTime = s.WorkShift.EndTime,
                        WorkShiftToleranceMinutes = s.WorkShift.ToleranceMinutes
                    }).ToList()
                }).ToListAsync();
        }

        public async Task<User?> FindIncludingRole(Expression<Func<User, bool>> predicate)
        {
            return await context.Employees.Where(predicate).Include(e => e.Role)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> FindAny(Expression<Func<User, bool>> predicate)
        {
            return await context.Employees.AnyAsync(predicate);
        }
        public async Task<User?> FindIncluding(Guid id)
        {
            return await context.Employees.Where(e => e.Id == id)
                .Include(e => e.EmployeeShifts).SingleOrDefaultAsync();
        }

        public async Task<User?> FindForPunchIn(Guid id)
        {
            return await context.Employees.Where(e => e.Id == id).Include(e => e.EmployeeFaces)
                .Include(e => e.Company).ThenInclude(c => c.CompanyGeolocation)
                .Include(e => e.EmployeeShifts)!.ThenInclude(es => es.WorkShift).FirstOrDefaultAsync();
        }

        public void Create(User user)
        {
            context.Employees.Add(user);
        }
        public async Task<TreatedUserInformationDto?> FindUserInformation(String email)
        {
            return await context.Employees.Where(e => e.Email == email).Select(e => new TreatedUserInformationDto()
            {
                Id = e.Id,
                Name = e.Name,
                Email = e.Email,
                RoleName = e.Role != null ? e.Role.Name : null,
                RoleId = e.RoleId,
                IsAllowedBypassCoord = e.IsAllowedBypassCoord,
                IsActive = e.IsActive,
                IsConfirmed = e.IsConfirmed,
                Password = e.Password!,
                CompanyId = e.CompanyId,
                CompanyName = e.Company.Name,
                EmployeeShift =
                    e.EmployeeShifts!.Where(es => es.IsActive == true)
                        .Select(es => new EmployeeShiftInformationDto()
                        {
                            StartDate = es.StartDate,
                            ShiftType = es.WorkShift!.ShiftType,
                            StartTime = es.WorkShift.StartTime,
                            EndTime = es.WorkShift.EndTime
                        }).FirstOrDefault(),
                LastPunchType = e.EmployeeAttendances!.OrderBy(ea => ea.CreatedAt).Last().PunchType,
                LastsEmployeeAttendances = e.EmployeeAttendances!.OrderByDescending(ea => ea.CreatedAt).Select(ea =>
                        new LastEmployeeAttendancesDto()
                        {
                            PunchType = ea.PunchType,
                            ShiftType = ea.ShiftType,
                            PunchTime = ea.PunchTime,
                            PunchDate = ea.CreatedAt
                        })
                    .Take(10)
                    .ToList()
            }).FirstOrDefaultAsync();
        }

        public void Update(User user)
        {
            context.Employees.Update(user);
        }

        public async Task<User?> Find(Expression<Func<User, bool>> predicate)
        {
            return await context
                .Employees
                .FirstOrDefaultAsync(predicate);
        }
    }
}
