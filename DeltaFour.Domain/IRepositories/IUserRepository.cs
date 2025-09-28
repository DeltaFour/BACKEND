using DeltaFour.Domain.Entities;

namespace DeltaFour.Domain.IRepositories
{
    public interface IUserRepository
    {
        Task Create(User user);
        Task Update(User user);
        Task<User?> GetUserById(Guid id);
        Task<User?> GetUserByEmail(string email);
         
    }
}
