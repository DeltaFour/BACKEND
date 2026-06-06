namespace DeltaFour.Domain.IRepositories;

public interface IUnitOfWork
{
    IUserRepository UserRepository { get; }
    IAddressRepository AddressRepository { get; }
    ICompanyGeolocationRepository CompanyGeolocationRepository { get; }
    IUserAuthRepository UserAuthRepository { get; }
    ICompanyRepository CompanyRepository { get; }
    IDepartmentRepository DepartmentRepository { get; }
    IRoleRepository RoleRepository { get; }
    IRolePermissionRepository RolePermissionRepository { get; }
    IActionRepository ActionRepository { get; }
    ILocationRepository LocationRepository { get; }
    IWorkShiftRepository WorkShiftRepository { get; }
    IUserAttendanceRepository UserAttendanceRepository { get; }
    IUserShiftRepository UserShiftRepository { get; }
    IUserFaceRepository UserFaceRepository { get; }
    ISubscriptionRepository SubscriptionRepository { get; }
    ISubscriptionEventRepository SubscriptionEventRepository { get; }
    ITimeSheetRepository TimeSheetRepository { get; }
    ITimeSheetSignatureRepository TimeSheetSignatureRepository { get; }
    ITimeSheetSignatureTokenRepository TimeSheetSignatureTokenRepository { get; }
    ITimeSheetAuditRepository TimeSheetAuditRepository { get; }
    IUserPunctualityMetricRepository UserPunctualityMetricRepository { get; }
    IClusterCentroidRepository ClusterCentroidRepository { get; }

    Task Save();
}
