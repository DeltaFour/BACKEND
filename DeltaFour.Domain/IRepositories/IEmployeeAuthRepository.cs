using DeltaFour.Domain.Entities;
using System.Linq.Expressions;

namespace DeltaFour.Domain.IRepositories
{
    public interface IEmployeeAuthRepository : IBaseRepository<UserAuth>
    {
        void Create(UserAuth userAuth);
        void Update(UserAuth userAuth);
        void Delete(UserAuth userAuth);
    }
}
