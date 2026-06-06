using DeltaFour.Domain.Entities;

namespace DeltaFour.Domain.IRepositories;

public interface ITimeSheetAuditRepository : IBaseRepository<TimeSheetAudit>
{
    Task<List<TimeSheetAudit>> FindByTimeSheet(Guid timeSheetId);

    void Create(TimeSheetAudit audit);
}
