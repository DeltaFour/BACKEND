namespace DeltaFour.Domain.Entities
{
    public class UserAuth
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public User? User { get; set; }

        public bool IsExpired()
        {
            return this.ExpiresAt.CompareTo(DateTime.UtcNow) > 0; 
        }
    }
}
