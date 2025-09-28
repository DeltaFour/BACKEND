using DeltaFour.Domain.Entities;

namespace DeltaFour.Domain.IRepositories
{
    public interface IWorkShiftRepository : IBaseRepository<WorkShift>
    {
        Task Create(WorkShift workShift);
        
        Task Update(WorkShift workShift);
        
        Task Delete(WorkShift workShift);
    }
}