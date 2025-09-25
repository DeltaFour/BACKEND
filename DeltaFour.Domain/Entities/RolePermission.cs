namespace DeltaFour.Domain.Entities
{
    public class RolePermission
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid RoleId { get; set; }
        public Guid ActionId { get; set; }
        public Guid LocationId { get; set; }
        public Role? Role { get; set; }
        public Action? Action { get; set; }
        public Location? Location { get; set; }
    }
}
