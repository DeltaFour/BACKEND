using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;

namespace DeltaFour.Infrastructure.Repositories
{
    public class AllRepositories(AppDbContext context) : IUnitOfWork
    {
        private IUserRepository? _userRepository;
        private IAddressRepository? _addressRepository;
        private ICompanyGeolocationRepository? _companyGeolocationRepository;
        private IUserAuthRepository? _userAuthRepository;
        private ICompanyRepository? _companyRepository;
        private IDepartmentRepository? _departmentRepository;
        private IRoleRepository? _roleRepository;
        private IRolePermissionRepository? _rolePermissionRepository;
        private IActionRepository? _actionRepository;
        private ILocationRepository? _locationRepository;
        private IWorkShiftRepository? _workShiftRepository;
        private IUserAttendanceRepository? _employeeAttendanceRepository;
        private IUserShiftRepository? _employeeShiftRepository;
        private IUserFaceRepository? _employeeFaceRepository;
        private ISubscriptionRepository? _subscriptionRepository;
        private ISubscriptionEventRepository? _subscriptionEventRepository;
        private ITimeSheetRepository? _timeSheetRepository;
        private ITimeSheetSignatureRepository? _timeSheetSignatureRepository;
        private ITimeSheetSignatureTokenRepository? _timeSheetSignatureTokenRepository;
        private ITimeSheetAuditRepository? _timeSheetAuditRepository;
        private IUserPunctualityMetricRepository? _userPunctualityMetricRepository;
        private IClusterCentroidRepository? _clusterCentroidRepository;
        private IPasswordResetTokenRepository? _passwordResetTokenRepository;
        private INotificationRepository? _notificationRepository;

        public IUserRepository UserRepository
        {
            get { return _userRepository ??= new UserRepository(context); }
        }

        public IAddressRepository AddressRepository
        {
            get { return _addressRepository ??= new AddressRepository(context); }
        }

        public ICompanyGeolocationRepository CompanyGeolocationRepository
        {
            get { return _companyGeolocationRepository ??= new CompanyGeolocationRepository(context); }
        }

        public IUserAuthRepository UserAuthRepository
        {
            get { return _userAuthRepository ??= new UserAuthRepository(context); }
        }

        public ICompanyRepository CompanyRepository
        {
            get { return _companyRepository ??= new CompanyRepository(context); }
        }

        public IDepartmentRepository DepartmentRepository
        {
            get { return _departmentRepository ??= new DepartmentRepository(context); }
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

        public ISubscriptionRepository SubscriptionRepository
        {
            get { return _subscriptionRepository ??= new SubscriptionRepository(context); }
        }

        public ISubscriptionEventRepository SubscriptionEventRepository
        {
            get { return _subscriptionEventRepository ??= new SubscriptionEventRepository(context); }
        }

        public ITimeSheetRepository TimeSheetRepository
        {
            get { return _timeSheetRepository ??= new TimeSheetRepository(context); }
        }

        public ITimeSheetSignatureRepository TimeSheetSignatureRepository
        {
            get { return _timeSheetSignatureRepository ??= new TimeSheetSignatureRepository(context); }
        }

        public ITimeSheetSignatureTokenRepository TimeSheetSignatureTokenRepository
        {
            get { return _timeSheetSignatureTokenRepository ??= new TimeSheetSignatureTokenRepository(context); }
        }

        public ITimeSheetAuditRepository TimeSheetAuditRepository
        {
            get { return _timeSheetAuditRepository ??= new TimeSheetAuditRepository(context); }
        }

        public IUserPunctualityMetricRepository UserPunctualityMetricRepository
        {
            get { return _userPunctualityMetricRepository ??= new UserPunctualityMetricRepository(context); }
        }

        public IClusterCentroidRepository ClusterCentroidRepository
        {
            get { return _clusterCentroidRepository ??= new ClusterCentroidRepository(context); }
        }

        public IPasswordResetTokenRepository PasswordResetTokenRepository
        {
            get { return _passwordResetTokenRepository ??= new PasswordResetTokenRepository(context); }
        }

        public INotificationRepository NotificationRepository
        {
            get { return _notificationRepository ??= new NotificationRepository(context); }
        }

        public async Task Save()
        {
            await context.SaveChangesAsync();
        }
    }
}
