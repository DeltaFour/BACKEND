using DeltaFour.Domain.Entities;

namespace DeltaFour.Domain.IRepositories
{
    public interface IUserFaceRepository : IBaseRepository<UserFace>
    {
        void Create(UserFace userFace);
        
        void Delete(UserFace userFace);
        
        void CreateAll(List<UserFace> users);
        
        void DeleteAll(List<UserFace> users);
    }
}