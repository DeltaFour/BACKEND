using DeltaFour.Domain.Entities;
using System.Linq.Expressions;

namespace DeltaFour.Domain.IRepositories
{
    public interface IUserAuthRepository : IBaseRepository<UserAuth>
    {
        void Create(UserAuth userAuth);
        void Update(UserAuth userAuth);
        void Delete(UserAuth userAuth);
    }
}
