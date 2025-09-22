using DeltaFour.Domain.Entities;

namespace DeltaFour.Domain.IRepositories
{
    public interface IUserAuthRepository
    {
        Task<UserAuth?> FindByUserId(Guid userId);
        Task<UserAuth?> GetById(Guid userId);
        Task Create(UserAuth userAuth);
        Task Update(UserAuth userAuth);
        Task Delete(UserAuth userAuth);
    }
}
