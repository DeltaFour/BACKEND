using Bogus;
using DeltaFour.Domain.Entities;
using DeltaFour.Domain.Enum;
using DeltaFour.Infrastructure.Context;

namespace DeltaFour.Test.Factories;

public class WorkShiftFactory(AppDbContext dbContext)
{
    private readonly Faker<WorkShift> _faker = new Faker<WorkShift>("pt_BR")
        .RuleFor(w => w.Id, _ => Guid.NewGuid())
        .RuleFor(w => w.ShiftType, f => f.PickRandom<ShiftType>())
        .RuleFor(w => w.StartTime, _ => new TimeOnly(8, 0))
        .RuleFor(w => w.EndTime, _ => new TimeOnly(17, 0))
        .RuleFor(w => w.ToleranceMinutes, f => f.Random.Int(5, 30))
        .RuleFor(w => w.CreatedBy, _ => Guid.NewGuid())
        .RuleFor(w => w.CreatedAt, f => f.Date.Past());

    public WorkShift Generate(Guid? companyId = null)
    {
        var workShift = _faker.Generate();
        workShift.CompanyId = companyId ?? Guid.NewGuid();
        return workShift;
    }

    public List<WorkShift> Generate(int count, Guid? companyId = null)
    {
        var workShifts = _faker.Generate(count);
        foreach (var workShift in workShifts)
        {
            workShift.CompanyId = companyId ?? Guid.NewGuid();
        }
        return workShifts;
    }

    public async Task<WorkShift> CreateAsync(Guid companyId, Action<WorkShift>? configure = null)
    {
        var workShift = Generate(companyId);
        configure?.Invoke(workShift);

        dbContext.WorkShifts.Add(workShift);
        await dbContext.SaveChangesAsync();

        return workShift;
    }

    public async Task<List<WorkShift>> CreateAsync(int count, Guid companyId, Action<WorkShift>? configure = null)
    {
        var workShifts = Generate(count, companyId);

        foreach (var workShift in workShifts)
        {
            configure?.Invoke(workShift);
        }

        dbContext.WorkShifts.AddRange(workShifts);
        await dbContext.SaveChangesAsync();

        return workShifts;
    }
}
