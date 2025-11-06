using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;

namespace DeltaFour.Infrastructure.Repositories
{
    public class AllRepositories(AppDbContext context) : IUnitOfWork
    {
        private IEmployeeRepository? _userRepository;
        private IEmployeeAuthRepository? _userAuthRepository;
        private ICompanyRepository? _companyRepository;
        private IRoleRepository? _roleRepository;
        private IRolePermissionRepository? _rolePermissionRepository;
        private IActionRepository? _actionRepository;
        private ILocationRepository? _locationRepository;
        private IWorkShiftRepository? _workShiftRepository;
        private IEmployeeAttendanceRepository? _employeeAttendanceRepository;
        private IEmployeeShiftRepository? _employeeShiftRepository;

        public IEmployeeRepository EmployeeRepository
        {
            get { return _userRepository ??= new EmployeeRepository(context); }
        }

        public IEmployeeAuthRepository EmployeeAuthRepository
        {
            get { return _userAuthRepository ??= new EmployeeAuthRepository(context); }
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

        public IEmployeeAttendanceRepository EmployeeAttendanceRepository
        {
            get { return _employeeAttendanceRepository ??= new EmployeeAttendanceRepository(context); }
        }

        public IEmployeeShiftRepository EmployeeShiftRepository
        {
            get { return _employeeShiftRepository ??= new EmployeeShiftRepository(context); }
        }

        public async Task Save()
        {
            await context.SaveChangesAsync();
        }
    }
}