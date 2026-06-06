using DeltaFour.Domain.Entities;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeltaFour.Infrastructure.Repositories
{
    public class AddressRepository(AppDbContext context) : IAddressRepository
    {

        public async Task<Address?> Find(Expression<Func<Address, bool>> predicate)
        {
            return await context.Address.FirstOrDefaultAsync(predicate);
        }
        public void Create(Address address)
        {
            context.Address.Add(address);
        }
        public void Update(Address address)
        {
            context.Address.Update(address);
        }
    }
}
