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
        Guid.Parse("ffffffff-0000-0000-0000-000000000006"),
        Guid.Parse("ffffffff-0000-0000-0000-000000000007"),
        Guid.Parse("ffffffff-0000-0000-0000-000000000008"),
        Guid.Parse("ffffffff-0000-0000-0000-000000000009"),
        Guid.Parse("ffffffff-0000-0000-0000-000000000010"),
        Guid.Parse("ffffffff-0000-0000-0000-000000000011"),
        Guid.Parse("ffffffff-0000-0000-0000-000000000012"),
        Guid.Parse("ffffffff-0000-0000-0000-000000000013"),
        Guid.Parse("ffffffff-0000-0000-0000-000000000014"),
        Guid.Parse("ffffffff-0000-0000-0000-000000000015"),
        Guid.Parse("ffffffff-0000-0000-0000-000000000016"),
        Guid.Parse("ffffffff-0000-0000-0000-000000000017"),
        Guid.Parse("ffffffff-0000-0000-0000-000000000018"),
        Guid.Parse("ffffffff-0000-0000-0000-000000000019"),
        Guid.Parse("ffffffff-0000-0000-0000-000000000020"),
        Guid.Parse("ffffffff-0000-0000-0000-000000000021"),
        Guid.Parse("ffffffff-0000-0000-0000-000000000022"),
        Guid.Parse("ffffffff-0000-0000-0000-000000000023"),
        Guid.Parse("ffffffff-0000-0000-0000-000000000024"),
        Guid.Parse("ffffffff-0000-0000-0000-000000000025"),
    ];

    private const string Password   = "#Admin@123";
    private static readonly Coordinates FakeCoord = new(-23.5505, -46.6333);

    [HttpPost("populate")]
    public async Task<IActionResult> Populate()
    {
        var alreadySeeded = await db.Companies.AnyAsync(c => c.Id == CompanyId);
        if (alreadySeeded)
            return Ok(new { message = "Seed já foi executado anteriormente. Nenhuma alteração feita." });

        await using var transaction = await db.Database.BeginTransactionAsync();

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

            var admin  = CreateUser(UserAdminId,  "Admin DeltaFour",    "admin@deltafourdemo.com",       RoleAdminId,    DeptRhId, "(11) 99000-0001", "111.444.777-35");
            var anaRh  = CreateUser(UserAnaId,    "Ana Oliveira",       "rh@deltafourdemo.com",          RoleRhId,       DeptRhId, "(11) 99801-2233", "529.982.247-25");
            var carlos = CreateUser(UserCarlosId, "Carlos Funcionário", "funcionario@deltafourdemo.com", RoleEmployeeId, DeptTiId, "(11) 97654-3210", "123.456.789-09");

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
                    SignedByEmployee = false,
                    EmployeeSignedAt = null,
                    SignedByHR       = false
                });

                attendances.AddRange(BuildAttendances(emp.Id));
            }

            await SaveOrFail("Folhas de ponto");

            db.EmployeeAttendances.AddRange(attendances);
            await SaveOrFail("Registros de presença");

            db.UserPunctualityMetrics.AddRange(
                employees.Select(emp => BuildMetric(emp.Id, attendances)));
            await SaveOrFail("Métricas de pontualidade");

            await transaction.CommitAsync();

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
                total_metricas   = employees.Count,
                mes              = "Maio/2026"
            });
        }
        catch (SeedStepException ex)
        {
            await transaction.RollbackAsync();

            return StatusCode(500, new
            {
                step  = ex.Step,
                error = Innermost(ex).Message
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
        Guid roleId, Guid deptId, string cellphone, string cpf) => new()
        {
            Id                    = id,
            CompanyId             = CompanyId,
            RoleId                = roleId,
            DepartmentId          = deptId,
            Name                  = name,
            Email                 = email,
            Password              = passwordService.Hash(Password),
            Cpf                   = cpf,
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
                Cpf                   = BuildCpf(faker.Random),
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

    /// <summary>
    /// Gera os registros de ponto de Maio/2026 para um usuário, com atrasos que
    /// variam tanto em FREQUÊNCIA (quantos dias) quanto em DURAÇÃO (quantos minutos).
    /// Cada usuário cai em um perfil (Pontual / Em Atenção / Crítico) sorteado de
    /// forma determinística, formando três grupos bem separados nos dois eixos do
    /// gráfico de dispersão usado pelo K-Means.
    /// </summary>
    private static List<UserAttendance> BuildAttendances(Guid userId)
    {
        var weekdays = Enumerable.Range(1, 31)
            .Where(d => new DateTime(2026, 5, d).DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday)
            .ToList();

        HashSet<int> absenceDays;
        Dictionary<int, int> lateMinutesByDay;

        if (userId == UserCarlosId)
        {
            // Perfil "Em Atenção" fixo e conhecido, útil para a demonstração.
            absenceDays      = [12, 19];
            lateMinutesByDay = new Dictionary<int, int> { [6] = 18, [13] = 23, [20] = 16, [27] = 27 };
        }
        else
        {
            var rng = new Random(userId.GetHashCode() & 0x7FFFFFFF);

            var shuffled = weekdays.OrderBy(_ => rng.Next()).ToList();
            absenceDays   = shuffled.Take(rng.Next(0, 3)).ToHashSet();
            var available = shuffled.Where(d => !absenceDays.Contains(d)).ToList();

            // Sorteia o perfil de pontualidade do colaborador.
            var profile = rng.NextDouble();
            int lateCount;
            int minLate, maxLate;

            if (profile < 0.45)          // ~45% Pontual: raramente atrasa, e pouco.
            {
                lateCount = rng.Next(0, 3);    // 0–2 dias
                minLate   = 5;
                maxLate   = 13;
            }
            else if (profile < 0.78)     // ~33% Em Atenção: atrasos moderados.
            {
                lateCount = rng.Next(4, 8);    // 4–7 dias
                minLate   = 14;
                maxLate   = 33;
            }
            else                          // ~22% Crítico: atrasos frequentes e longos.
            {
                lateCount = rng.Next(9, 15);   // 9–14 dias
                minLate   = 35;
                maxLate   = 90;
            }

            lateMinutesByDay = available
                .Take(Math.Min(lateCount, available.Count))
                .ToDictionary(d => d, _ => rng.Next(minLate, maxLate + 1));
        }

        var punches = new List<UserAttendance>();

        for (int day = 1; day <= 31; day++)
        {
            var date = new DateTime(2026, 5, day);
            if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) continue;
            if (absenceDays.Contains(day)) continue;

            bool isLate = lateMinutesByDay.TryGetValue(day, out int lateMin);
            TimeOnly? timeLate = isLate ? new TimeOnly(lateMin / 60, lateMin % 60) : null;

            // Hora de entrada reflete o atraso (8h + atraso); minutos podem passar de 59.
            var inTime = new DateTime(2026, 5, day, 8, 0, 0, DateTimeKind.Utc).AddMinutes(lateMin);

            punches.Add(Punch(userId, inTime,                                              PunchType.IN,  isLate, timeLate));
            punches.Add(Punch(userId, new DateTime(2026, 5, day, 12, 0, 0, DateTimeKind.Utc), PunchType.OUT, false,  null));
            punches.Add(Punch(userId, new DateTime(2026, 5, day, 13, 0, 0, DateTimeKind.Utc), PunchType.IN,  false,  null));
            punches.Add(Punch(userId, new DateTime(2026, 5, day, 17, 0, 0, DateTimeKind.Utc), PunchType.OUT, false,  null));
        }

        return punches;
    }

    /// <summary>
    /// Calcula as métricas de pontualidade a partir dos pontos gerados no seed,
    /// espelhando a lógica de <c>PunctualityMetricsService.RecalculateMetricsForUser</c>.
    /// O cluster é fixado em 0 para que a evolução fique visível ao rodar o K-Means.
    /// </summary>
    private static UserPunctualityMetric BuildMetric(Guid userId, List<UserAttendance> attendances)
    {
        var inPunches = attendances
            .Where(a => a.UserId == userId && a.PunchType == PunchType.IN)
            .ToList();

        int totalAttendances     = inPunches.Count;
        int totalLateAttendances = inPunches.Count(a => a.IsLate);

        double latePercentage = totalAttendances > 0
            ? Math.Round((double)totalLateAttendances / totalAttendances * 100, 2)
            : 0;

        var lateMinutes = inPunches
            .Where(a => a.IsLate && a.TimeLate.HasValue)
            .Select(a => a.TimeLate!.Value.Hour * 60 + a.TimeLate.Value.Minute)
            .ToList();

        double averageLateMinutes = lateMinutes.Count > 0 ? Math.Round(lateMinutes.Average(), 2) : 0;
        int    maxLateMinutes     = lateMinutes.Count > 0 ? lateMinutes.Max() : 0;

        var workedDates = inPunches.Select(a => a.PunchTime.Date).Distinct().ToHashSet();

        int totalAbsences = Enumerable.Range(1, 31)
            .Select(d => new DateTime(2026, 5, d))
            .Count(d => d.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday
                        && !workedDates.Contains(d.Date));

        return new UserPunctualityMetric
        {
            UserId               = userId,
            TotalAttendances     = totalAttendances,
            TotalLateAttendances = totalLateAttendances,
            LatePercentage       = latePercentage,
            AverageLateMinutes   = averageLateMinutes,
            MaxLateMinutes       = maxLateMinutes,
            TotalAbsences        = totalAbsences,
            TotalWorkedDays      = workedDates.Count,
            Cluster              = 0,
            LastCalculatedAt     = DateTime.UtcNow,
            UpdatedAt            = DateTime.UtcNow
        };
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

    /// <summary>
    /// Gera um CPF válido (com dígitos verificadores) já formatado como 000.000.000-00.
    /// </summary>
    private static string BuildCpf(Bogus.Randomizer rng)
    {
        var n = new int[9];
        for (int i = 0; i < 9; i++) n[i] = rng.Number(0, 9);

        int d1 = CpfCheckDigit(n, 10);
        int d2 = CpfCheckDigit([.. n, d1], 11);

        var digits = string.Concat(n) + d1 + d2;
        return $"{digits[..3]}.{digits[3..6]}.{digits[6..9]}-{digits[9..]}";
    }

    private static int CpfCheckDigit(int[] digits, int startWeight)
    {
        int sum = 0;
        for (int i = 0; i < digits.Length; i++)
            sum += digits[i] * (startWeight - i);

        int remainder = sum % 11;
        return remainder < 2 ? 0 : 11 - remainder;
    }

    private static Exception Innermost(Exception ex)
    {
        while (ex.InnerException is not null)
            ex = ex.InnerException;
        return ex;
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
