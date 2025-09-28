using Microsoft.Identity.Client;
using OpenFlowWebServer.Domain;
using OpenFlowWebServer.Domain.Entities;
using OpenFlowWebServer.Enums;
using OpenFlowWebServer.Repository;

namespace OpenFlowWebServer.Services
{
    public interface IDeployProjectServices
    {
        // Define methods and properties for deploying projects here
        public Task DeployProjectAsync(Guid projectId);
        public Task ClearProjectAsync(Guid projectId);
    }

    public class DeployProjectServices : IDeployProjectServices
    {
        private readonly IModelRepository _modelRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IParametersListServices _parametersListServices;
        private readonly IQueueRepository _queueRepository;
        private string queueName;

        public DeployProjectServices(IModelRepository modelRepository, IProjectRepository projectRepository, IParametersListServices parametersListServices, IQueueRepository queueRepository)
        {
            _modelRepository = modelRepository;
            _projectRepository = projectRepository;
            _parametersListServices = parametersListServices;
            _queueRepository = queueRepository;
        }

        public async Task DeployProjectAsync(Guid projectId)
        {
           var project = await _projectRepository.GetByIdAsync(projectId);
           queueName = $"ProjectId{project.Id.ToString()}Parameters";
           var modelsParametersTask = new List<Task>(project.Models.Count);
           foreach (var model in project.Models)
           {
                modelsParametersTask.Add( _parametersListServices.CreateList(model.Id));
           }
           var modelList = project.Models.ToList();
            //var modelsParameters =await Task.WhenAll(modelsParametersTask.ToArray());
            var paramTasks = modelList.Select(m => _parametersListServices.CreateList(m.Id)).ToArray();
           var modelsParameters = await Task.WhenAll(paramTasks);
           int configurationCounts = 0;
           if (project.ChooseMethod == ProjectDeployModels.DF.ToString())
            {
              var result = DatasetFirst(project, modelsParameters);
                configurationCounts = result.Count;
                await Task.WhenAll(result.ToArray());
            }
            else if (project.ChooseMethod == ProjectDeployModels.MF.ToString())
            {
                ModelFirst();
            }
            else
            {
                throw new NotSupportedException($"Choose method '{project.ChooseMethod}' is not supported.");
            }
            
            _projectRepository.DeployModel(projectId,configurationCounts);
//            throw new NotImplementedException();
        }

        public async Task ClearProjectAsync(Guid projectId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            await _projectRepository.ResetProject(projectId);
            await _queueRepository.DeleteQueueAsync($"ProjectId{project.Id.ToString()}Parameters");
        }


        private ICollection<Task> DatasetFirst(Project project ,ICollection<ModelParameters> modelParametersCollection)
        {
            var tasks = new List<Task>();
            foreach(var model in project.Models)
            {
                foreach (var dataset in project.Datasets)
                {
                    foreach (var modelParameters in modelParametersCollection)
                    {
                        foreach(var parameter in modelParameters.ParametersList)
                        {
                            var copiedParameter = new Dictionary<string, object>(parameter);
                            copiedParameter.Add("ModelId", model.Id);
                            copiedParameter.Add("DatasetId", dataset.Id);
                            tasks.Add(_queueRepository.EnqueueAsync(copiedParameter, queueName));
                        }
                    }
                }
            }

            return tasks;
        }

        private void ModelFirst()
        {
            throw new NotImplementedException();
        }

    }
}
