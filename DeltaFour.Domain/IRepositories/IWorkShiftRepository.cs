using DeltaFour.Domain.Entities;

namespace DeltaFour.Domain.IRepositories
{
    public interface IWorkShiftRepository : IBaseRepository<WorkShift>
    {
        void Create(WorkShift workShift);
        
        void Update(WorkShift workShift);
        
        void Delete(WorkShift workShift);

        Task<WorkShift?> GetByTimeAndEmployeeId(DateTime timePunch, Guid employeeId, Guid companyId);
    }
}