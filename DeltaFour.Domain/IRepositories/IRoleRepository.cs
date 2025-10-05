using DeltaFour.Domain.Entities;

namespace DeltaFour.Domain.IRepositories
{
    public interface IRoleRepository : IBaseRepository<Role>
    {
        void Create(Role role);
        
        void Update(Role role);
        
        void Delete(Role role);
    }
}
