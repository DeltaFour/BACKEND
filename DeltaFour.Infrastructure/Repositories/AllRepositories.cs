using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;

namespace DeltaFour.Infrastructure.Repositories
{
    public class AllRepositories(AppDbContext context)
    {
        private IUserRepository? _userRepository;
        private IUserAuthRepository? _userAuthRepository;
        private ICompanyRepository? _companyRepository;
        private IRoleRepository? _roleRepository;

        public IUserRepository UserRepository
        {
            get { return _userRepository ??= new UserRepository(context); }
        }

        public IUserAuthRepository UserAuthRepository
        {
            get { return _userAuthRepository ??= new UserAuthRepository(context); }
        }

        public ICompanyRepository CompanyRepository
        {
            get { return _companyRepository ??= new CompanyRepository(context); }
        }

        public IRoleRepository RoleRepository
        {
            get { return _roleRepository ??= new RoleRepository(context); } 
        }
    }
}
