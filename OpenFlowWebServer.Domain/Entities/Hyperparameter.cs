namespace OpenFlowWebServer.Domain.Entities
{
    public class Hyperparameter
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ModelId { get; set; }
        public string? Type { get; set; }

        public string? Description { get; set; }

        // Navigation property
        public virtual Model Model { get; set; }
        public string Name { get; set; }
        public decimal bottomRange { get; set; }
        public decimal upperRange { get; set; }
        public decimal step { get; set; }

    }
}
