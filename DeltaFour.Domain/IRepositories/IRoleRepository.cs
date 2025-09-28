using DeltaFour.Domain.Entities;

namespace DeltaFour.Domain.IRepositories
{
    public interface IRoleRepository : IBaseRepository<Role>
    {
        Task Create(Role role);
        
        Task Update(Role role);
        
        Task Delete(Role role);
    }
}
