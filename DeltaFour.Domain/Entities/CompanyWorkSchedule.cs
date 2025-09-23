namespace DeltaFour.Domain.Entities
{
    public class CompanyWorkSchedule: BaseEntity
    {
        public Guid CompanyId { get; set; }
        
        public DateTime StartTime { get; set; }
        
        public DateTime EndTime { get; set; }
        
        public int ToleranceMinutes { get; set; }
        
        public Guid? UpdatedBy { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
        
        public Guid CreatedBy { get; set; }
        
        public Company? Company { get; set; }  
    }
}
