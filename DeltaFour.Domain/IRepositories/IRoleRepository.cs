using DeltaFour.Domain.Entities;
using System.Linq.Expressions;

namespace DeltaFour.Domain.IRepositories
{
    public interface IRoleRepository : IBaseRepository<Role>
    {
        void Create(Role role);
        
        void Update(Role role);
        
        void Delete(Role role);
        
        Task<Boolean> FindAny(Expression<Func<Role, bool>> predicate);
    }
}
