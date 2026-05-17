using System.Linq.Expressions;
using System.Collections.Generic;

namespace DeltaFour.Domain.IRepositories
{
    public interface IBaseRepository<T>
    {
        Task<T?> Find(Expression<Func<T, bool>> predicate);

        Task<List<T>> FindAll(Expression<Func<T, bool>> predicate)
        {
            return Task.FromResult(new List<T>());
        }

        void CreateRange(IEnumerable<T> entities)
        {
            throw new System.NotImplementedException();
        }
    }
}