using DeltaFour.Domain.Entities;
using System.Linq.Expressions;

namespace DeltaFour.Domain.IRepositories
{
    public interface IDepartmentRepository : IBaseRepository<Department>
    {
        Task<List<TResult>> FindAll<TResult>(Expression<Func<Department, bool>> predicate, Expression<Func<Department, TResult>> selector);
        Task<bool> HasUsers(Guid departmentId);
        void Create(Department department);
        void Update(Department department);
        void Delete(Department department);
    }
}
