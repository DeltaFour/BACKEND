using DeltaFour.Domain.Entities;

namespace DeltaFour.Domain.IRepositories
{
    public interface IUserRepository :  IBaseRepository<User>
    {
        Task Create(User user);
        Task Update(User user);
         
    }
}
