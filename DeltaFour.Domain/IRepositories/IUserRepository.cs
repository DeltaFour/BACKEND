using DeltaFour.Application.Dtos;
using DeltaFour.Domain.Entities;
using DeltaFour.Domain.ValueObjects.Dtos;
using System.Linq.Expressions;

namespace DeltaFour.Domain.IRepositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<PagedResponse<UserResponseDto>> GetAll(
            Guid companyId, string? search, string? roleName, string? departmentName, int page, int pageSize);

        Task<Boolean> FindAny(Expression<Func<User, bool>> predicate);

        Task<User?> FindIncluding(Guid id);

        Task<User?> FindForPunchIn(Guid id);

        Task<User?> FindIncludingRole(Expression<Func<User, bool>> predicate);

        void Create(User user);

        Task<TreatedUserInformationDto?> FindUserInformation(String email);

        void Update(User user);

        Task<User?> FindByEmailForPunch(String email);

        Task<List<User>> GetRhUsers(Guid companyId);

        Task<PagedResponse<AllAttendanceByCompanyResponse>> GetAllAttendanceByCompany(
            Guid companyId, string? search, DateTime? date, string? punchType,
            bool? isLate, bool sortDesc, int page, int pageSize);

        Task<List<User>> GetAllSelect(Guid companyId);
        
        Task<List<User>> GetDashboardUsers(Guid companyId);
        
        Task<List<UserAttendance>> GetDashboardAttendances(Guid companyId, DateTime startDate, DateTime endDate);
    }
}