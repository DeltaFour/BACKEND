using DeltaFour.Domain.Entities;

namespace DeltaFour.Domain.IRepositories
{
    public interface IUserAuthRepository : IBaseRepository<UserAuth>
    {
        void Create(UserAuth userAuth);
        void Update(UserAuth userAuth);
        void Delete(UserAuth userAuth);
    }
}
