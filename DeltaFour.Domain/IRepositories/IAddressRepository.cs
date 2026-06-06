using DeltaFour.Domain.Entities;

namespace DeltaFour.Domain.IRepositories
{
    public interface IAddressRepository :IBaseRepository<Address>
    {
        void Create(Address address);
        
        void Update(Address address);
    }
}
