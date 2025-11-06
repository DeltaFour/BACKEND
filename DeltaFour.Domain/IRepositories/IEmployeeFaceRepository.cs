using DeltaFour.Domain.Entities;

namespace DeltaFour.Domain.IRepositories
{
    public interface IEmployeeFaceRepository : IBaseRepository<EmployeeFace>
    {
        void Create(EmployeeFace employeeFace);
        
        void Delete(EmployeeFace employeeFace);
        
        void CreateAll(List<EmployeeFace> employees);
        
        void DeleteAll(List<EmployeeFace> employees);
    }
}