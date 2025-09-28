using DeltaFour.Domain.Entities;

namespace DeltaFour.Domain.IRepositories
{
    public interface IEmployeeFaceRepository : IBaseRepository<EmployeeFace>
    {
        Task Create(EmployeeFace employeeFace);
        
        Task Delete(EmployeeFace employeeFace);
        
        Task CreateAll(List<EmployeeFace> employees);
        
        Task DeleteAll(List<EmployeeFace> employees);
    }
}