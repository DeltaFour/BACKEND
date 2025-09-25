namespace DeltaFour.Domain.Entities
{
    public class Action
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Name { get; set; }
    }
}
