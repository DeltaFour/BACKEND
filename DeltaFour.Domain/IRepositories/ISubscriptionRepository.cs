using DeltaFour.Domain.Entities;

namespace DeltaFour.Domain.IRepositories;

public interface ISubscriptionRepository : IBaseRepository<Subscription>
{
    void Create(Subscription subscription);
}
