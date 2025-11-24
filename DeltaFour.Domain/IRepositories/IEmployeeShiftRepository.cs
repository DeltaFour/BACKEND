using DeltaFour.Domain.Entities;
using System.Linq.Expressions;

namespace DeltaFour.Domain.IRepositories
{
    public interface IEmployeeShiftRepository : IBaseRepository<UserShift>
    {
        void Create(UserShift userAttendance);
        
        void CreateAll(List<UserShift> employeeShifts);
        
        void Update(UserShift userAttendance);
        
        void Delete(UserShift userAttendance);

        Task<Boolean> FindAny(Expression<Func<UserShift, bool>> predicate);
        
        void UpdateAll(List<UserShift> employeeShifts);
        
        void DeleteAll(List<UserShift> employeeShifts);
    }
}