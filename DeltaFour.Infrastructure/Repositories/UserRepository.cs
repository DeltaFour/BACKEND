using DeltaFour.Application.Dtos;
using DeltaFour.Domain.Entities;
using DeltaFour.Domain.Enum;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Domain.ValueObjects.Dtos;
using DeltaFour.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeltaFour.Infrastructure.Repositories
{
    public class UserRepository(AppDbContext context) : IUserRepository
    {
        public async Task<PagedResponse<UserResponseDto>> GetAll(
            Guid companyId, string? search, string? roleName, string? departmentName, int page, int pageSize)
        {
            var query = context.Employees.Where(e => e.IsActive && e.CompanyId == companyId);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(e => e.Name.Contains(search) || e.Email.Contains(search));

            if (!string.IsNullOrWhiteSpace(roleName) && roleName != "all")
                query = query.Where(e => e.Role != null && e.Role.Name == roleName);

            if (!string.IsNullOrWhiteSpace(departmentName) && departmentName != "all")
                query = query.Where(e => e.Department != null && e.Department.Name == departmentName);

            var total = await query.CountAsync();

            var data = await query
                .OrderBy(e => e.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new UserResponseDto()
                {
                    Id = e.Id,
                    Name = e.Name,
                    Cellphone = e.Cellphone,
                    Email = e.Email,
                    RoleName = e.Role!.Name,
                    IsActive = e.IsActive,
                    IsAllowedBypassCoord = e.IsAllowedBypassCoord,
                    LastLogin = e.LastLogin,
                    DepartmentName = e.Department != null ? e.Department.Name : null,
                    ShiftDto = e.UserShifts!.Select(s => new UserResponseShiftsDto()
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

            return new PagedResponse<UserResponseDto>
            {
                Data = data,
                Total = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)total / pageSize)
            };
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
                .Include(e => e.UserShifts).SingleOrDefaultAsync();
        }

        public async Task<User?> FindForPunchIn(Guid id)
        {
            return await context.Employees.Where(e => e.Id == id).Include(e => e.UserFaces)
                .Include(e => e.Company).ThenInclude(c => c.CompanyGeolocation)
                .Include(e => e.UserShifts)!.ThenInclude(es => es.WorkShift).FirstOrDefaultAsync();
        }

        public async Task<User?> FindByEmailForPunch(String email)
        {
            return await context.Employees.Where(e => e.Email == email).Include(e => e.UserFaces)
                .Include(e => e.Company).ThenInclude(c => c.CompanyGeolocation)
                .Include(e => e.UserShifts)!.ThenInclude(es => es.WorkShift).FirstOrDefaultAsync();
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
                IsAllowedBypassFace = e.IsAllowedBypassFacial,
                IsActive = e.IsActive,
                IsConfirmed = e.IsConfirmed,
                MustChangePassword = e.MustChangePassword,
                Password = e.Password!,
                CompanyId = e.CompanyId,
                CompanyName = e.Company.Name,
                UserShift =
                    e.UserShifts!.Where(es => es.IsActive == true)
                        .Select(es => new UserShiftInformationDto()
                        {
                            StartDate = es.StartDate,
                            ShiftType = es.WorkShift!.ShiftType,
                            StartTime = es.WorkShift.StartTime,
                            EndTime = es.WorkShift.EndTime
                        }).FirstOrDefault(),
                LastPunchType = e.UserAttendances!.OrderBy(ea => ea.CreatedAt).Last().PunchType,
                LastsUserAttendances = e.UserAttendances!.OrderByDescending(ea => ea.CreatedAt).Select(ea =>
                        new LastUserAttendancesDto()
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

        public async Task<List<User>> GetRhUsers(Guid companyId)
        {
            return await context.Employees.Where(e => e.CompanyId == companyId && e.Role.Name.Equals("RH"))
                .ToListAsync();
        }

        public async Task<User?> Find(Expression<Func<User, bool>> predicate)
        {
            return await context
                .Employees
                .FirstOrDefaultAsync(predicate);
        }

        public async Task<PagedResponse<AllAttendanceByCompanyResponse>> GetAllAttendanceByCompany(
            Guid companyId, string? search, DateTime? date, string? punchType,
            bool? isLate, bool sortDesc, int page, int pageSize)
        {
            var query = context.EmployeeAttendances
                .Join(context.Employees,
                      at => at.UserId,
                      e => e.Id,
                      (at, e) => new { at, e })
                .Where(x => x.e.IsActive && x.e.CompanyId == companyId);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x => x.e.Name.Contains(search));

            if (date.HasValue)
                query = query.Where(x => x.at.PunchTime >= date.Value.Date
                                      && x.at.PunchTime < date.Value.Date.AddDays(1));

            if (!string.IsNullOrWhiteSpace(punchType) && Enum.TryParse<PunchType>(punchType, true, out var pt))
                query = query.Where(x => x.at.PunchType == pt);

            if (isLate.HasValue)
                query = query.Where(x => x.at.IsLate == isLate.Value);

            var total = await query.CountAsync();

            query = sortDesc
                ? query.OrderByDescending(x => x.at.PunchTime)
                : query.OrderBy(x => x.at.PunchTime);

            var data = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new AllAttendanceByCompanyResponse
                {
                    AttendanceId = x.at.Id,
                    Name = x.e.Name,
                    TimePunched = x.at.PunchTime,
                    IsLate = x.at.IsLate,
                    Type = x.at.PunchType,
                    ShiftType = x.at.ShiftType,
                    Status = x.at.Status,
                    Justification = x.at.Justification,
                    Observation = x.at.Observation,
                    FilePath = x.at.FilePath,
                })
                .ToListAsync();

            return new PagedResponse<AllAttendanceByCompanyResponse>
            {
                Data = data,
                Total = total,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)total / pageSize)
            };
        }

        public Task<List<User>> GetAllSelect(Guid companyId)
        {
            return context.Employees.Where(e => e.IsActive == true && e.CompanyId == companyId)
                .Select(e => new User()
                {
                    Id = e.Id,
                    Name = e.Name,
                    Department = new Department
                    {
                        Name = e.Department != null ? e.Department.Name : null
                    }
                }).ToListAsync();
        }

        public async Task<List<User>> GetDashboardUsers(Guid companyId)
        {
            return await context.Employees
                .Where(e => e.CompanyId == companyId)
                .AsNoTracking()
                .Include(e => e.UserShifts)!
                .ThenInclude(s => s.WorkShift)
                .ToListAsync();
        }

        public async Task<List<UserAttendance>> GetDashboardAttendances(
            Guid companyId,
            DateTime startDate,
            DateTime endDate)
        {
            return await context.EmployeeAttendances
                .Where(a => a.User != null &&
                            a.User.CompanyId == companyId &&
                            a.User.IsActive &&
                            a.PunchTime >= startDate &&
                            a.PunchTime < endDate)
                .AsNoTracking()
                .Include(a => a.User)
                .ToListAsync();
        }
    }
}
