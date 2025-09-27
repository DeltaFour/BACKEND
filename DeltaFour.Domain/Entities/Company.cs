namespace DeltaFour.Domain.Entities
{
    public class Company : BaseEntity
    {
        public Guid? AddressId { get; set; }
        
        public string? Name { get; set; }
        
        public string? Cnpj {  get; set; }
        
        public bool IsActive { get; set; }
        
        public DateTime? UpdatedAt { get; set; }

        public Address? Address { get; set; }
        
        public List<Employee>? Employees { get; set; }
        
        public List<Role>? Roles { get; set; }
        
        public CompanyGeolocation? CompanyGeolocation { get; set; }
    }
}