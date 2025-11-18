using DeltaFour.Domain.Entities;
using DeltaFour.Domain.ValueObjects.Dtos;
using System.Linq.Expressions;

namespace DeltaFour.Domain.IRepositories
{
    public interface IWorkShiftRepository : IBaseRepository<WorkShift>
    {
        void Create(WorkShift workShift);
        
        void Update(WorkShift workShift);
        
        void Delete(WorkShift workShift);
        
        Task<List<WorkShiftResponseDto>> FindAll(Expression<Func<WorkShift, bool>> predicate);

        Task<WorkShiftPunchDto?> GetByTimeAndEmployeeId(TimeOnly timePunch, Guid employeeId, Guid companyId);
    }
}