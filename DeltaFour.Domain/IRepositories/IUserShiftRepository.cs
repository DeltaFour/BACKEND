using DeltaFour.Domain.Entities;
using System.Linq.Expressions;

namespace DeltaFour.Domain.IRepositories
{
    public interface IUserShiftRepository : IBaseRepository<UserShift>
    {
        void Create(UserShift userAttendance);
        
        void CreateAll(List<UserShift> userShifts);
        
        void Update(UserShift userAttendance);
        
        void Delete(UserShift userAttendance);

        Task<Boolean> FindAny(Expression<Func<UserShift, bool>> predicate);
        
        void UpdateAll(List<UserShift> userShifts);
        
        void DeleteAll(List<UserShift> userShifts);
    }
}