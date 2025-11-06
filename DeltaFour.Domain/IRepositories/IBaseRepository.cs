using System.Linq.Expressions;

namespace DeltaFour.Domain.IRepositories
{
    public interface IBaseRepository<T>
    {
        Task<T?> Find(Expression<Func<T, bool>> predicate);

        Task<List<T>> FindAll(Expression<Func<List<T>, bool>> predicate)
        {
            return Task.FromResult(new List<T>());
        }
    }
}