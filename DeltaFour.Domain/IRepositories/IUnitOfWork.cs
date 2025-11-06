namespace DeltaFour.Domain.IRepositories;

public interface IUnitOfWork
{
    IEmployeeRepository EmployeeRepository { get; }
    IEmployeeAuthRepository EmployeeAuthRepository { get; }
    ICompanyRepository CompanyRepository { get; }
    IRoleRepository RoleRepository { get; }
    IRolePermissionRepository RolePermissionRepository { get; }
    IActionRepository ActionRepository { get; }
    ILocationRepository LocationRepository { get; }
    IWorkShiftRepository WorkShiftRepository { get; }
    IEmployeeAttendanceRepository EmployeeAttendanceRepository { get; }
    IEmployeeShiftRepository EmployeeShiftRepository { get; }

    Task Save();
}
