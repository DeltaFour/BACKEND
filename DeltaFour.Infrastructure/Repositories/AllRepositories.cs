using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;

namespace DeltaFour.Infrastructure.Repositories
{
    public class AllRepositories(AppDbContext context) : IUnitOfWork
    {
        private IUserRepository? _userRepository;
        private IUserAuthRepository? _userAuthRepository;
        private ICompanyRepository? _companyRepository;
        private IRoleRepository? _roleRepository;
        private IRolePermissionRepository? _rolePermissionRepository;
        private IActionRepository? _actionRepository;
        private ILocationRepository? _locationRepository;
        private IWorkShiftRepository? _workShiftRepository;
        private IUserAttendanceRepository? _employeeAttendanceRepository;
        private IUserShiftRepository? _employeeShiftRepository;
        private IUserFaceRepository? _employeeFaceRepository;

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

        public IRolePermissionRepository RolePermissionRepository
        {
            get { return _rolePermissionRepository ??= new RolePermissionRepository(context); }
        }

        public IActionRepository ActionRepository
        {
            get { return _actionRepository ??= new ActionRepository(context); }
        }

        public ILocationRepository LocationRepository
        {
            get { return _locationRepository ??= new LocationRepository(context); }
        }

        public IWorkShiftRepository WorkShiftRepository
        {
            get { return _workShiftRepository ??= new WorkShiftRepository(context); }
        }

        public IUserAttendanceRepository UserAttendanceRepository
        {
            get { return _employeeAttendanceRepository ??= new UserAttendanceRepository(context); }
        }

        public IUserShiftRepository UserShiftRepository
        {
            get { return _employeeShiftRepository ??= new UserShiftRepository(context); }
        }

        public IUserFaceRepository UserFaceRepository
        {
            get { return _employeeFaceRepository ??= new UserFaceRepository(context); }
        }

        public async Task Save()
        {
            await context.SaveChangesAsync();
        }
    }
}