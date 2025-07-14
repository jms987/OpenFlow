namespace OpenFlowWebServer.Domain.Entities
{
    public class Project
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ProjectName { get; set; }
        public string? ProjectDescription { get; set; }
        public string? ChooseMethod { get; set; }
        public virtual ICollection<Dataset> Datasets { get; set; } = new List<Dataset>();
        public virtual ICollection<Model> Models { get; set; } = new List<Model>();
    }
}
