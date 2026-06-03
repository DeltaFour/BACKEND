using Bogus;
using DeltaFour.Domain.Entities;
using DeltaFour.Domain.Enum;
using DeltaFour.Infrastructure.Context;

namespace DeltaFour.Test.Factories;

public class UserAttendanceFactory(AppDbContext dbContext)
{
    private readonly Faker<UserAttendance> _faker = new Faker<UserAttendance>("pt_BR")
        .RuleFor(a => a.Id, _ => Guid.NewGuid())
        .RuleFor(a => a.PunchTime, f => f.Date.Recent(30))
        .RuleFor(a => a.PunchType, _ => PunchType.IN)
        .RuleFor(a => a.ShiftType, f => f.PickRandom<ShiftType>())
        .RuleFor(a => a.Coord, _ => new Coordinates(-23.5505, -46.6333))
        .RuleFor(a => a.IsLate, _ => false)
        .RuleFor(a => a.TimeLate, _ => null)
        .RuleFor(a => a.CreatedBy, _ => Guid.NewGuid())
        .RuleFor(a => a.CreatedAt, f => f.Date.Past());

    public UserAttendance Generate(Guid userId, bool isLate = false, int? lateMinutes = null)
    {
        var attendance = _faker.Generate();
        attendance.UserId = userId;
        attendance.IsLate = isLate;

        if (isLate && lateMinutes.HasValue)
        {
            attendance.TimeLate = new TimeOnly(lateMinutes.Value / 60, lateMinutes.Value % 60);
        }

        return attendance;
    }

    public List<UserAttendance> Generate(int count, Guid userId, bool isLate = false, int? lateMinutes = null)
    {
        var attendances = new List<UserAttendance>();
        for (int i = 0; i < count; i++)
        {
            attendances.Add(Generate(userId, isLate, lateMinutes));
        }
        return attendances;
    }

    public async Task<UserAttendance> CreateAsync(Guid userId, Action<UserAttendance>? configure = null)
    {
        var attendance = Generate(userId);
        configure?.Invoke(attendance);

        dbContext.EmployeeAttendances.Add(attendance);
        await dbContext.SaveChangesAsync();

        return attendance;
    }

    public async Task<List<UserAttendance>> CreateAsync(int count, Guid userId, Action<UserAttendance>? configure = null)
    {
        var attendances = new List<UserAttendance>();

        for (int i = 0; i < count; i++)
        {
            var attendance = Generate(userId);
            configure?.Invoke(attendance);
            attendances.Add(attendance);
        }

        dbContext.EmployeeAttendances.AddRange(attendances);
        await dbContext.SaveChangesAsync();

        return attendances;
    }

    public async Task<UserAttendance> CreateLateAsync(Guid userId, int lateMinutes, Action<UserAttendance>? configure = null)
    {
        var attendance = Generate(userId, isLate: true, lateMinutes: lateMinutes);
        configure?.Invoke(attendance);

        dbContext.EmployeeAttendances.Add(attendance);
        await dbContext.SaveChangesAsync();

        return attendance;
    }

    public async Task<List<UserAttendance>> CreateLateAsync(int count, Guid userId, int lateMinutes, Action<UserAttendance>? configure = null)
    {
        var attendances = new List<UserAttendance>();

        for (int i = 0; i < count; i++)
        {
            var attendance = Generate(userId, isLate: true, lateMinutes: lateMinutes);
            configure?.Invoke(attendance);
            attendances.Add(attendance);
        }

        dbContext.EmployeeAttendances.AddRange(attendances);
        await dbContext.SaveChangesAsync();

        return attendances;
    }
}
