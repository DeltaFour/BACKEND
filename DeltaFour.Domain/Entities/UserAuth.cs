namespace DeltaFour.Domain.Entities
{
    public class UserAuth(Guid employeeId, string? refreshToken, DateTime expiresAt)
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public Guid EmployeeId { get; set; } = employeeId;
        
        public string? RefreshToken { get; set; } = refreshToken;
        
        public DateTime ExpiresAt { get; set; } = expiresAt;
        
        public User? Employee { get; set; }

        public bool IsExpired()
        {
            return this.ExpiresAt.CompareTo(DateTime.UtcNow) > 0; 
        }
    }
}