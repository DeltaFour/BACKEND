using DeltaFour.Domain.Entities;
using DeltaFour.Domain.Enum;
using DeltaFour.Infrastructure.Repositories;

namespace DeltaFour.Infrastructure.Seeders
{
    public class RoleSeeder(AllRepositories repository)
    {
        private static readonly Guid companyId = Guid.Parse(Environment.GetEnvironmentVariable("SUPER_ADMIN_ID"));
        
        private async Task<bool> RolesAlreadyExists()
        {
            return await repository.RoleRepository.FindAny(e => e.Name.Equals(nameof(RoleType.RH)) || e.Name.Equals(nameof(RoleType.EMPLOYEE)));
        }

        private void SaveRoles()
        {
            Role rh =  new Role()
            {
                CompanyId = companyId,
                Name = nameof(RoleType.RH),
                IsActive = true,
            };
            Role employee = new Role()
            {
                CompanyId = companyId,
                Name = nameof(RoleType.EMPLOYEE),
                IsActive = true,
            };
            Role admin = new Role()
            {
                CompanyId = companyId,
                Name = nameof(RoleType.ADMIN),
                IsActive = true,
            };
            repository.RoleRepository.Create(rh);
            repository.RoleRepository.Create(employee);
            repository.RoleRepository.Create(admin);
        }
        
        public async Task SeedAsync()
        {
            var exists = await RolesAlreadyExists();

            if (exists) return;

            SaveRoles();

            await repository.Save();
        }
    }
}
