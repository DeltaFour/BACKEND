using DeltaFour.Domain.Entities;
using System.Linq.Expressions;

namespace DeltaFour.Domain.IRepositories
{
    public interface IUserAuthRepository : IBaseRepository<UserAuth>
    {
        Task Create(UserAuth userAuth);
        Task Update(UserAuth userAuth);
        Task Delete(UserAuth userAuth);
    }
}
