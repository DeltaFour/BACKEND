namespace DeltaFour.Domain.IRepositories;

public interface IUnitOfWork
{
    IUserRepository UserRepository { get; }
    IUserAuthRepository UserAuthRepository { get; }
    ICompanyRepository CompanyRepository { get; }
    IRoleRepository RoleRepository { get; }
    IRolePermissionRepository RolePermissionRepository { get; }
    IActionRepository ActionRepository { get; }
    ILocationRepository LocationRepository { get; }
    IWorkShiftRepository WorkShiftRepository { get; }
    IUserAttendanceRepository UserAttendanceRepository { get; }
    IUserShiftRepository UserShiftRepository { get; }

    Task Save();
}
