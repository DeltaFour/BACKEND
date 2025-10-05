using DeltaFour.Domain.Entities;

namespace DeltaFour.Domain.IRepositories
{
    public interface IRolePermissionRepository : IBaseRepository<RolePermission>
    {
        void Create(RolePermission rolePermission);
        void Delete(RolePermission rolePermission);
        void DeleteAll(List<RolePermission> rolePermissions);
    }
}
