using DeltaFour.Application.Services;
using DeltaFour.Domain.Entities;
using DeltaFour.Domain.Enum;
using DeltaFour.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeltaFour.API.Controllers;

[Route("api/v1/seed")]
[ApiController]
public class SeedController(AppDbContext db, IPasswordService passwordService) : ControllerBase
{
    private static readonly Guid CompanyId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001");
    private static readonly Guid RoleRhId = Guid.Parse("bbbbbbbb-0000-0000-0000-000000000001");
    private static readonly Guid RoleEmployeeId = Guid.Parse("bbbbbbbb-0000-0000-0000-000000000002");
    private static readonly Guid WorkShiftId = Guid.Parse("cccccccc-0000-0000-0000-000000000001");
    private static readonly Guid UserRhId = Guid.Parse("dddddddd-0000-0000-0000-000000000001");
    private static readonly Guid UserEmployeeId = Guid.Parse("dddddddd-0000-0000-0000-000000000002");
    private static readonly Guid UserShiftId = Guid.Parse("eeeeeeee-0000-0000-0000-000000000001");

    private static readonly TimeOnly ShiftStart = new(8, 0);
    private static readonly TimeOnly ShiftEnd = new(17, 0);

    private const int ToleranceMin = 10;

    private static readonly Coordinates FakeCoord = new(-23.5631, -46.6565);

    [HttpPost("populate")]
    public async Task<IActionResult> Populate()
    {
        try
        {
            var alreadySeeded = await db.Companies.AnyAsync(c => c.Id == CompanyId);

            if (alreadySeeded)
            {
                return Ok(new
                {
                    message = "Seed já foi executado anteriormente. Nenhuma alteração feita."
                });
            }

            var company = CreateCompany();
            db.Companies.Add(company);
            await SaveOrFail("Empresa");

            db.Roles.AddRange(
                CreateRole(RoleRhId, nameof(RoleType.RH)),
                CreateRole(RoleEmployeeId, nameof(RoleType.EMPLOYEE))
            );
            await SaveOrFail("Roles");

            db.WorkShifts.Add(CreateWorkShift());
            await SaveOrFail("WorkShift");

            var userRh = CreateUser(
                UserRhId,
                "Ana RH",
                "rh@deltafourdemo.com",
                RoleRhId,
                "senha123");

            var userEmployee = CreateUser(
                UserEmployeeId,
                "Carlos Funcionário",
                "funcionario@deltafourdemo.com",
                RoleEmployeeId,
                "senha123");

            db.Employees.AddRange(userRh, userEmployee);
            await SaveOrFail("Usuários");

            db.EmployeeShifts.Add(
                CreateUserShift(
                    UserShiftId,
                    UserEmployeeId,
                    WorkShiftId));

            await SaveOrFail("UserShift");

            db.TimeSheets.Add(new TimeSheet
            {
                UserId = UserEmployeeId,
                Month = 5,
                Year = 2026,
                SignedByEmployee = false,
                SignedByHR = false
            });

            await SaveOrFail("TimeSheet");

            var attendances = BuildMayAttendances(UserEmployeeId);

            db.EmployeeAttendances.AddRange(attendances);

            await SaveOrFail("Attendances");

            return Ok(new
            {
                message = "Seed executado com sucesso!",
                empresa = company.Name,
                usuarioRh = new
                {
                    id = userRh.Id,
                    email = userRh.Email
                },
                usuarioFunc = new
                {
                    id = userEmployee.Id,
                    email = userEmployee.Email
                },
                senha = "senha123",
                totalPontos = attendances.Count,
                faltas = 2,
                atrasos = 4,
                mes = "Maio/2026"
            });
        }
        catch (SeedStepException ex)
        {
            return StatusCode(500, new
            {
                step = ex.Step,
                error = ex.InnerException?.Message ?? ex.Message,
                innerInner = ex.InnerException?.InnerException?.Message
            });
        }
    }

    private async Task SaveOrFail(string step)
    {
        try
        {
            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new SeedStepException(step, ex);
        }
    }

    private sealed class SeedStepException(string step, Exception inner)
        : Exception($"Falha na etapa: {step}", inner)
    {
        public string Step { get; } = step;
    }

    private static Company CreateCompany() => new()
    {
        Id = CompanyId,
        Name = "Empresa Demo DeltaFour",
        LegalName = "Empresa Demo DeltaFour LTDA",
        Cnpj = "00.000.000/0001-00",
        IsActive = true,
        CreatedBy = Guid.Empty
    };

    private static Role CreateRole(Guid id, string name) => new()
    {
        Id = id,
        CompanyId = CompanyId,
        Name = name,
        IsActive = true,
        CreatedBy = Guid.Empty
    };

    private static WorkShift CreateWorkShift() => new()
    {
        Id = WorkShiftId,
        CompanyId = CompanyId,
        ShiftType = ShiftType.Diurno,
        StartTime = ShiftStart,
        EndTime = ShiftEnd,
        ToleranceMinutes = ToleranceMin,
        CreatedBy = Guid.Empty
    };

    private User CreateUser(
        Guid id,
        string name,
        string email,
        Guid roleId,
        string plainPassword) => new()
        {
            Id = id,
            CompanyId = CompanyId,
            RoleId = roleId,
            Name = name,
            Email = email,
            Password = passwordService.Hash(plainPassword),
            IsActive = true,
            IsConfirmed = true,
            IsAllowedBypassCoord = true,
            IsAllowedBypassFacial = true,
            CreatedBy = Guid.Empty
        };

    private static UserShift CreateUserShift(
        Guid id,
        Guid userId,
        Guid shiftId) => new()
        {
            Id = id,
            UserId = userId,
            ShiftId = shiftId,
            IsActive = true,
            StartDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            CreatedBy = Guid.Empty
        };

    private static List<UserAttendance> BuildMayAttendances(Guid userId)
    {
        var absenceDays = new HashSet<int> { 12, 19 };
        var lateDays = new HashSet<int> { 6, 13, 20, 27 };

        var punches = new List<UserAttendance>();

        for (int day = 1; day <= 31; day++)
        {
            var date = new DateTime(2026, 5, day, 0, 0, 0, DateTimeKind.Utc);

            if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                continue;

            if (absenceDays.Contains(day))
                continue;

            var isLate = lateDays.Contains(day);
            var lateMin = isLate ? 25 : 0;
            TimeOnly? timeLate = isLate ? new TimeOnly(0, lateMin) : null;

            punches.Add(MakePunch(
                userId,
                new DateTime(2026, 5, day, 8, lateMin, 0, DateTimeKind.Utc),
                PunchType.IN,
                isLate,
                timeLate));

            punches.Add(MakePunch(
                userId,
                new DateTime(2026, 5, day, 12, 0, 0, DateTimeKind.Utc),
                PunchType.OUT,
                false,
                null));

            punches.Add(MakePunch(
                userId,
                new DateTime(2026, 5, day, 13, 0, 0, DateTimeKind.Utc),
                PunchType.IN,
                false,
                null));

            punches.Add(MakePunch(
                userId,
                new DateTime(2026, 5, day, 17, 0, 0, DateTimeKind.Utc),
                PunchType.OUT,
                false,
                null));
        }

        return punches;
    }

    private static UserAttendance MakePunch(
        Guid userId,
        DateTime punchTime,
        PunchType punchType,
        bool isLate,
        TimeOnly? timeLate) => new()
        {
            UserId = userId,
            PunchTime = punchTime,
            PunchType = punchType,
            ShiftType = ShiftType.Diurno,
            Coord = FakeCoord,
            IsLate = isLate,
            TimeLate = timeLate,
            Status = StatusAttendance.aprovado.ToString(),
            CreatedBy = UserRhId
        };
}