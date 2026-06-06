using DeltaFour.Domain.Entities;

namespace DeltaFour.Domain.IRepositories;

public interface ITimeSheetSignatureTokenRepository : IBaseRepository<TimeSheetSignatureToken>
{
    Task<TimeSheetSignatureToken?> FindByToken(string token);

    Task<List<TimeSheetSignatureToken>> FindByTimeSheet(Guid timeSheetId);

    void Create(TimeSheetSignatureToken token);

    void Update(TimeSheetSignatureToken token);
}
