using System.Linq.Expressions;

namespace DeltaFour.Domain.IRepositories
{
    public interface IBaseRepository<T>
    {
        Task<T?> Find(Expression<Func<T, bool>> predicate);
    }
}
