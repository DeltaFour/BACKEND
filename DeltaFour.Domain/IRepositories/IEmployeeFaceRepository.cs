using DeltaFour.Domain.Entities;

namespace DeltaFour.Domain.IRepositories
{
    public interface IEmployeeFaceRepository : IBaseRepository<UserFace>
    {
        void Create(UserFace userFace);
        
        void Delete(UserFace userFace);
        
        void CreateAll(List<UserFace> employees);
        
        void DeleteAll(List<UserFace> employees);
    }
}