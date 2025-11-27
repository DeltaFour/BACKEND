using System.Text.Json.Serialization;

namespace DeltaFour.Domain.Entities
{
    public class Role : BaseEntity
    {
        public Guid CompanyId { get; set; }
        
        public string? Name { get; set; }
        
        public bool IsActive { get; set; }
        
        public Guid CreatedBy { get; set; }
        
        public Guid? UpdatedBy { get; set; }
        
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public Company? Company { get; set; }
        
        [JsonIgnore]
        public List<User>? User { get; set; }
        
        public List<RolePermission>? RolePermissions { get; set; }
    }
}
