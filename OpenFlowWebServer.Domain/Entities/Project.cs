namespace OpenFlowWebServer.Domain.Entities
{
    public class Project
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ProjectName { get; set; }
        public string? ProjectDescription { get; set; }
        public string? ChooseMethod { get; set; }
        public bool IsDeployed { get; set; } = false;
        public int InitialConfigurations { get; set; } = 0;
        public virtual ICollection<Dataset> Datasets { get; set; } = new List<Dataset>();
        public virtual ICollection<Model> Models { get; set; } = new List<Model>();
        public virtual ICollection<Security> Securities { get; set; } = new List<Security>();
    }
}
