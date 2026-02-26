using DeltaFour.Domain.Entities;
using DeltaFour.Domain.ValueObjects.Dtos;
using System.Linq.Expressions;
using System.Linq.Expressions;

namespace DeltaFour.Domain.IRepositories
{
    public interface IUserRepository :  IBaseRepository<User>
    {
        Task<List<UserResponseDto>> GetAll(Guid companyId);
        
        Task<Boolean> FindAny(Expression<Func<User, bool>> predicate);
        
        Task<User?> FindIncluding(Guid id);

        Task<User?> FindForPunchIn(Guid id);

        Task<User?> FindIncludingRole(Expression<Func<User, bool>> predicate);
        
        void Create(User user);

        Task<TreatedUserInformationDto?> FindUserInformation(String email);
        
        void Update(User user);

    }
}