using DeltaFour.Domain.Entities;

namespace DeltaFour.Domain.IRepositories
{
    public interface IRolePermissionRepository : IBaseRepository<RolePermission>
    {
        Task Create(RolePermission rolePermission);
        Task Delete(RolePermission rolePermission);
        Task DeleteAll(List<RolePermission> rolePermissions);
    }
}
