namespace DeltaFour.Domain.Entities
{
    public class UserAuth(Guid UserId, string? refreshToken, DateTime expiresAt)
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public Guid UserId { get; set; } = UserId;
        
        public string? RefreshToken { get; set; } = refreshToken;
        
        public DateTime ExpiresAt { get; set; } = expiresAt;
        
        public User? User { get; set; }

        public bool IsExpired()
        {
            return this.ExpiresAt.CompareTo(DateTime.UtcNow) > 0; 
        }
    }
}