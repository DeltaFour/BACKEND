using DeltaFour.Domain.Entities;
using DeltaFour.Domain.ValueObjects.Dtos;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace DeltaFour.Domain.IRepositories
{
    public interface IWorkShiftRepository : IBaseRepository<WorkShift>
    {
        void Create(WorkShift workShift);
        void CreateRange(IEnumerable<WorkShift> workShifts);

        void Update(WorkShift workShift);

        void Delete(WorkShift workShift);

        Task<List<WorkShiftResponseDto>> FindAll(Expression<Func<WorkShift, bool>> predicate);

        Task<WorkShiftPunchDto?> GetByUserIdAndIsActive(Guid userId, Guid companyId);
    }
}