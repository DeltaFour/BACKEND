using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;

namespace DeltaFour.Infrastructure.Repositories
{
    public class AllRepositories(AppDbContext context)
    {
        private IUserRepository? _userRepository;
        private IUserAuthRepository? _userAuthRepository;

        public IUserRepository UserRepository
        {
            get { return _userRepository ??= new UserRepository(context); }
        }

        public IUserAuthRepository UserAuthRepository
        {
            get { return _userAuthRepository ??= new UserAuthRepository(context); }
        }
    }
}
