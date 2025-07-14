namespace OpenFlowWebServer.Domain.Entities
{
    public class Model
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ModelName { get; set; }
        public Guid ProjectId { get; set; }
        public string? ModelDescription { get; set; }
        public string? ModelType { get; set; }
        public virtual ICollection<Hyperparameter> Hyperparameters { get; set; } = new List<Hyperparameter>();

    }
}
