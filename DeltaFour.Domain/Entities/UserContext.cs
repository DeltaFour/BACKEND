namespace DeltaFour.Domain.Entities
{
    public class UserContext
    {
        public Guid Id { get; set; }
        
        public string? Name { get; set; }
    
        public string? Email { get; set; }
    
        public Guid? RoleId { get; set; }
        
        public Guid CompanyId { get; set; }
        
        public bool IsAllowedBypassCoord {  get; set; }
    }
}
