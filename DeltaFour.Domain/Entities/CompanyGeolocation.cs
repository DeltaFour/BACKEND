using System.Drawing;

namespace DeltaFour.Domain.Entities
{
    public class CompanyGeolocation : BaseEntity
    {
        public Guid CompanyId { get; set; } 
        
        public Point Coordinates { get; set; }
        
        public int RadiusMeters { get; set; }
        
        public bool IsActive { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
        
        public Guid? UpdatedBy { get; set; }
        
        public Guid CreatedBy { get; set; }
        
        public Company? Company { get; set; }
    }
}
