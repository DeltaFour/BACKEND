using DeltaFour.Domain.Entities;

namespace DeltaFour.Domain.IRepositories;

public interface ISubscriptionEventRepository : IBaseRepository<SubscriptionEvent>
{
    void Create(SubscriptionEvent subscriptionEvent);
}
