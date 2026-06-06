using Bogus;
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
    private static readonly Guid AddressId          = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000000");
    private static readonly Guid CompanyId          = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001");

    private static readonly Guid RoleAdminId        = Guid.Parse("bbbbbbbb-0000-0000-0000-000000000001");
    private static readonly Guid RoleRhId           = Guid.Parse("bbbbbbbb-0000-0000-0000-000000000002");
    private static readonly Guid RoleEmployeeId     = Guid.Parse("bbbbbbbb-0000-0000-0000-000000000003");

    private static readonly Guid WorkShiftDiurnoId  = Guid.Parse("cccccccc-0000-0000-0000-000000000001");
    private static readonly Guid WorkShiftMatinoId  = Guid.Parse("cccccccc-0000-0000-0000-000000000002");

    private static readonly Guid DeptRhId           = Guid.Parse("de000000-0000-0000-0000-000000000001");
    private static readonly Guid DeptTiId           = Guid.Parse("de000000-0000-0000-0000-000000000002");
    private static readonly Guid DeptFinanceiroId   = Guid.Parse("de000000-0000-0000-0000-000000000003");
    private static readonly Guid DeptComercialId    = Guid.Parse("de000000-0000-0000-0000-000000000004");
    private static readonly Guid DeptOperacoesId    = Guid.Parse("de000000-0000-0000-0000-000000000005");

    private static readonly Guid UserAdminId        = Guid.Parse("dddddddd-0000-0000-0000-000000000001");
    private static readonly Guid UserAnaId          = Guid.Parse("dddddddd-0000-0000-0000-000000000002");
    private static readonly Guid UserCarlosId       = Guid.Parse("dddddddd-0000-0000-0000-000000000003");

    private static readonly Guid[] BogusUserIds =
    [
        Guid.Parse("ffffffff-0000-0000-0000-000000000001"),
        Guid.Parse("ffffffff-0000-0000-0000-000000000002"),
        Guid.Parse("ffffffff-0000-0000-0000-000000000003"),
        Guid.Parse("ffffffff-0000-0000-0000-000000000004"),
        Guid.Parse("ffffffff-0000-0000-0000-000000000005"),
    ];

    private const string Password   = "#Admin@123";
    private static readonly Coordinates FakeCoord = new(-23.5505, -46.6333);

    [HttpPost("populate")]
    public async Task<IActionResult> Populate()
    {
        var alreadySeeded = await db.Companies.AnyAsync(c => c.Id == CompanyId);
        if (alreadySeeded)
            return Ok(new { message = "Seed já foi executado anteriormente. Nenhuma alteração feita." });

        try
        {
            db.Address.Add(new Address
            {
                Id         = AddressId,
                Street     = "Avenida Paulista",
                Number     = 1000,
                Complement = "Conjunto 42",
                District   = "Bela Vista",
                City       = "São Paulo",
                State      = "SP",
                ZipCode    = "01310-100"
            });
            await SaveOrFail("Endereço");

            var company = new Company
            {
                Id          = CompanyId,
                AddressId   = AddressId,
                Name        = "DeltaFour Sistemas",
                LegalName   = "DeltaFour Sistemas e Tecnologia Ltda",
                Cnpj        = "12.345.678/0001-90",
                IsActive    = true,
                CreatedBy   = Guid.Empty
            };
            db.Companies.Add(company);
            await SaveOrFail("Empresa");

            db.Departments.AddRange(
                Dept(DeptRhId,         "Recursos Humanos"),
                Dept(DeptTiId,         "Tecnologia da Informação"),
                Dept(DeptFinanceiroId, "Financeiro"),
                Dept(DeptComercialId,  "Comercial"),
                Dept(DeptOperacoesId,  "Operações")
            );
            await SaveOrFail("Departamentos");

            db.Roles.AddRange(
                new Role { Id = RoleAdminId,    CompanyId = CompanyId, Name = nameof(RoleType.ADMIN),    IsActive = true, CreatedBy = Guid.Empty },
                new Role { Id = RoleRhId,       CompanyId = CompanyId, Name = nameof(RoleType.RH),       IsActive = true, CreatedBy = Guid.Empty },
                new Role { Id = RoleEmployeeId, CompanyId = CompanyId, Name = nameof(RoleType.EMPLOYEE), IsActive = true, CreatedBy = Guid.Empty }
            );
            await SaveOrFail("Cargos");

            db.WorkShifts.AddRange(
                new WorkShift
                {
                    Id               = WorkShiftDiurnoId,
                    CompanyId        = CompanyId,
                    ShiftType        = ShiftType.Diurno,
                    StartTime        = new TimeOnly(8, 0),
                    EndTime          = new TimeOnly(17, 0),
                    ToleranceMinutes = 10,
                    CreatedBy        = Guid.Empty
                },
                new WorkShift
                {
                    Id               = WorkShiftMatinoId,
                    CompanyId        = CompanyId,
                    ShiftType        = ShiftType.Matutino,
                    StartTime        = new TimeOnly(6, 0),
                    EndTime          = new TimeOnly(14, 0),
                    ToleranceMinutes = 10,
                    CreatedBy        = Guid.Empty
                }
            );
            await SaveOrFail("Turnos");

            var admin  = CreateUser(UserAdminId,  "Admin DeltaFour",    "admin@deltafourdemo.com",       RoleAdminId,    DeptRhId, "(11) 99000-0001");
            var anaRh  = CreateUser(UserAnaId,    "Ana Oliveira",       "rh@deltafourdemo.com",          RoleRhId,       DeptRhId, "(11) 99801-2233");
            var carlos = CreateUser(UserCarlosId, "Carlos Funcionário", "funcionario@deltafourdemo.com", RoleEmployeeId, DeptTiId, "(11) 97654-3210");

            var bogusUsers = GenerateBogusUsers();

            var allUsers = new[] { admin, anaRh, carlos }.Concat(bogusUsers).ToList();
            db.Employees.AddRange(allUsers);
            await SaveOrFail("Usuários");

            db.EmployeeShifts.Add(BuildUserShift(UserAdminId,  WorkShiftDiurnoId));
            db.EmployeeShifts.Add(BuildUserShift(UserAnaId,    WorkShiftDiurnoId));
            db.EmployeeShifts.Add(BuildUserShift(UserCarlosId, WorkShiftDiurnoId));

            for (int i = 0; i < bogusUsers.Count; i++)
                db.EmployeeShifts.Add(BuildUserShift(bogusUsers[i].Id, i % 3 == 2 ? WorkShiftMatinoId : WorkShiftDiurnoId));

            await SaveOrFail("Atribuição de turnos");

            var employees   = new[] { carlos }.Concat(bogusUsers).ToList();
            var attendances = new List<UserAttendance>();

            foreach (var emp in employees)
            {
                db.TimeSheets.Add(new TimeSheet
                {
                    UserId           = emp.Id,
                    Month            = 5,
                    Year             = 2026,
                    SignedByEmployee = emp.Id == UserCarlosId,
                    EmployeeSignedAt = emp.Id == UserCarlosId ? DateTime.UtcNow.AddDays(-2) : null,
                    SignedByHR       = false
                });

                attendances.AddRange(BuildAttendances(emp.Id));
            }

            await SaveOrFail("Folhas de ponto");

            db.EmployeeAttendances.AddRange(attendances);
            await SaveOrFail("Registros de presença");

            return Ok(new
            {
                message          = "Seed executado com sucesso!",
                empresa          = company.Name,
                usuarios_fixos   = new[]
                {
                    new { nome = admin.Name,  email = admin.Email,  role = "ADMIN"    },
                    new { nome = anaRh.Name,  email = anaRh.Email,  role = "RH"       },
                    new { nome = carlos.Name, email = carlos.Email, role = "EMPLOYEE" }
                },
                usuarios_gerados = bogusUsers.Select(u => new { u.Name, u.Email }).ToArray(),
                senha_todos      = Password,
                total_pontos     = attendances.Count,
                mes              = "Maio/2026"
            });
        }
        catch (SeedStepException ex)
        {
            return StatusCode(500, new
            {
                step  = ex.Step,
                error = ex.InnerException?.Message ?? ex.Message
            });
        }
    }

    private static Department Dept(Guid id, string name) => new()
    {
        Id        = id,
        CompanyId = CompanyId,
        Name      = name,
        CreatedBy = Guid.Empty
    };

    private User CreateUser(
        Guid id, string name, string email,
        Guid roleId, Guid deptId, string cellphone) => new()
        {
            Id                    = id,
            CompanyId             = CompanyId,
            RoleId                = roleId,
            DepartmentId          = deptId,
            Name                  = name,
            Email                 = email,
            Password              = passwordService.Hash(Password),
            Cellphone             = cellphone,
            IsActive              = true,
            IsConfirmed           = true,
            IsAllowedBypassCoord  = true,
            IsAllowedBypassFacial = true,
            CreatedBy             = Guid.Empty
        };

    private List<User> GenerateBogusUsers()
    {
        var faker = new Faker("pt_BR") { Random = new Randomizer(42) };

        var deptPool = new[] { DeptTiId, DeptFinanceiroId, DeptComercialId, DeptOperacoesId };

        var users = new List<User>();
        for (int i = 0; i < BogusUserIds.Length; i++)
        {
            var firstName = faker.Name.FirstName();
            var lastName  = faker.Name.LastName();
            var email     = $"{Normalize(firstName)}.{Normalize(lastName)}{i + 1}@deltafourdemo.com";

            users.Add(new User
            {
                Id                    = BogusUserIds[i],
                CompanyId             = CompanyId,
                RoleId                = RoleEmployeeId,
                DepartmentId          = deptPool[i % deptPool.Length],
                Name                  = $"{firstName} {lastName}",
                Email                 = email,
                Password              = passwordService.Hash(Password),
                Cellphone             = faker.Phone.PhoneNumber("(##) 9####-####"),
                IsActive              = true,
                IsConfirmed           = true,
                IsAllowedBypassCoord  = true,
                IsAllowedBypassFacial = true,
                CreatedBy             = Guid.Empty
            });
        }

        return users;
    }

    private static UserShift BuildUserShift(Guid userId, Guid shiftId) => new()
    {
        UserId    = userId,
        ShiftId   = shiftId,
        IsActive  = true,
        StartDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        CreatedBy = Guid.Empty
    };

    private static List<UserAttendance> BuildAttendances(Guid userId)
    {
        HashSet<int> absenceDays;
        HashSet<int> lateDays;

        if (userId == UserCarlosId)
        {
            absenceDays = [12, 19];
            lateDays    = [6, 13, 20, 27];
        }
        else
        {
            var rng = new Random(userId.GetHashCode() & 0x7FFFFFFF);
            var weekdays = Enumerable.Range(1, 31)
                .Where(d => new DateTime(2026, 5, d).DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday)
                .OrderBy(_ => rng.Next())
                .ToList();

            absenceDays = weekdays.Take(rng.Next(0, 3)).ToHashSet();
            lateDays    = weekdays.Where(d => !absenceDays.Contains(d)).Take(rng.Next(0, 5)).ToHashSet();
        }

        var punches = new List<UserAttendance>();

        for (int day = 1; day <= 31; day++)
        {
            var date = new DateTime(2026, 5, day);
            if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) continue;
            if (absenceDays.Contains(day)) continue;

            var isLate  = lateDays.Contains(day);
            var lateMin = isLate ? 25 : 0;
            TimeOnly? timeLate = isLate ? new TimeOnly(0, lateMin) : null;

            punches.Add(Punch(userId, new DateTime(2026, 5, day,  8, lateMin, 0, DateTimeKind.Utc), PunchType.IN,  isLate, timeLate));
            punches.Add(Punch(userId, new DateTime(2026, 5, day, 12,       0, 0, DateTimeKind.Utc), PunchType.OUT, false,  null));
            punches.Add(Punch(userId, new DateTime(2026, 5, day, 13,       0, 0, DateTimeKind.Utc), PunchType.IN,  false,  null));
            punches.Add(Punch(userId, new DateTime(2026, 5, day, 17,       0, 0, DateTimeKind.Utc), PunchType.OUT, false,  null));
        }

        return punches;
    }

    private static UserAttendance Punch(
        Guid userId, DateTime punchTime, PunchType type,
        bool isLate, TimeOnly? timeLate) => new()
        {
            UserId    = userId,
            PunchTime = punchTime,
            PunchType = type,
            ShiftType = ShiftType.Diurno,
            Coord     = FakeCoord,
            IsLate    = isLate,
            TimeLate  = timeLate,
            Status    = StatusAttendance.aprovado.ToString(),
            CreatedBy = UserAnaId
        };

    private async Task SaveOrFail(string step)
    {
        try   { await db.SaveChangesAsync(); }
        catch (Exception ex) { throw new SeedStepException(step, ex); }
    }

    private static string Normalize(string name)
    {
        var normalized = name.Normalize(System.Text.NormalizationForm.FormD);
        var sb = new System.Text.StringBuilder();
        foreach (var c in normalized)
        {
            var cat = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
            if (cat != System.Globalization.UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }
        return sb.ToString()
                 .Normalize(System.Text.NormalizationForm.FormC)
                 .ToLower()
                 .Replace(' ', '.');
    }

    private sealed class SeedStepException(string step, Exception inner)
        : Exception($"Falha na etapa: {step}", inner)
    {
        public string Step { get; } = step;
    }
}
