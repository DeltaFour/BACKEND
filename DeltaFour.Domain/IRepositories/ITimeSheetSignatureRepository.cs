using DeltaFour.Domain.Entities;

namespace DeltaFour.Domain.IRepositories;

public interface ITimeSheetSignatureRepository : IBaseRepository<TimeSheetSignature>
{
    Task<List<TimeSheetSignature>> FindByTimeSheet(Guid timeSheetId);

    void Create(TimeSheetSignature signature);
}
