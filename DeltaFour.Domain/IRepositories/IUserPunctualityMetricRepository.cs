using DeltaFour.Domain.Entities;

namespace DeltaFour.Domain.IRepositories
{
    public interface IUserPunctualityMetricRepository : IBaseRepository<UserPunctualityMetric>
    {
        void Create(UserPunctualityMetric metric);

        void Update(UserPunctualityMetric metric);

        Task<UserPunctualityMetric?> GetByUserId(Guid userId);

        Task<List<UserPunctualityMetric>> GetAllByCompanyId(Guid companyId);

        Task<List<UserPunctualityMetric>> GetAll();

        Task UpdateCluster(Guid userId, int cluster);

        Task UpdateClusterBatch(List<(Guid UserId, int Cluster)> updates);
    }
}
