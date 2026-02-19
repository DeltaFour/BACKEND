namespace DeltaFour.Domain.Entities
{
    public class UserFace(Guid UserId, string faceTemplate, Guid createdBy) : BaseEntity
    {
        public Guid UserId { get; set; } = UserId;

        public string FaceTemplate { get; set; } = faceTemplate;

        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public Guid? UpdatedBy { get; set; }
        
        public Guid CreatedBy { get; set; } = createdBy;

        public User? User { get; set; }

    }
}