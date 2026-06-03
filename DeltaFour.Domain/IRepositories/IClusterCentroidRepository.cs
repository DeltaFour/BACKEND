using DeltaFour.Domain.Entities;

namespace DeltaFour.Domain.IRepositories
{
    public interface IClusterCentroidRepository : IBaseRepository<ClusterCentroid>
    {
        void Create(ClusterCentroid centroid);

        void CreateRange(IEnumerable<ClusterCentroid> centroids);

        Task<List<ClusterCentroid>> GetByCompanyId(Guid companyId);

        Task DeleteByCompanyId(Guid companyId);

        Task ReplaceForCompany(Guid companyId, List<ClusterCentroid> centroids);
    }
}
