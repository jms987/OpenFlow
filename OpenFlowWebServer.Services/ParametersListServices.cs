using OpenFlowWebServer.Domain;
using OpenFlowWebServer.Domain.Entities;
using OpenFlowWebServer.Enums;

namespace OpenFlowWebServer.Services
{
    public interface IParametersListServices
    {
        /*Task<List<Dictionary<string, decimal>>> CreateList(Guid modelId);*/
        Task<ModelParameters> CreateList(Guid modelId);
    }

    public class ParametersListServices : IParametersListServices
    {
        private readonly IModelRepository _modelRepository;

        public ParametersListServices(IModelRepository modelRepository)
        {
            _modelRepository = modelRepository;
        }

        public async Task<ModelParameters> CreateList(Guid modelId)
        {
            var returnModel = new ModelParameters();
            Model model;
            try
            {
                model = await _modelRepository.GetByIdAsync(modelId);
            }
            catch (ArgumentNullException e)
            {
                throw e;
            }

            /*if (model == null)
            {
                throw new ArgumentNullException("Model not found.");
            }*/

            if (model.Hyperparameters.Count == 0)
            {
                throw new ArgumentException("Model has no hyperparameters defined.");
            }

            if (model.SearchMethod == ModelSearchMethod.GS.ToString())
            {
                returnModel.ParametersList = GridSearch(model.Hyperparameters);
            }
            else if (model.SearchMethod == ModelSearchMethod.RS.ToString())
            {
                returnModel.ParametersList = RandomSearch(model.Hyperparameters, model.Steps ?? 100);
            }
            else
            {
                throw new NotSupportedException($"Search method '{model.SearchMethod}' is not supported.");
            }

            return returnModel;
        }

        private List<Dictionary<string, object>> RandomSearch(ICollection<Hyperparameter> hyperparameters, int steps)
        {
            var list = GridSearch(hyperparameters);
            if (list.Count <= steps)
            {
                return list;
            }

            List<Dictionary<string, object>> randomList = new List<Dictionary<string, object>>();
            for (int i = 0; i < steps; i++)
            {
                var random = new Random();
                var randomIndex = random.Next(0, list.Count);
                var randomValue = list[randomIndex];
                list.RemoveAt(randomIndex);
                randomList.Add(randomValue);
            }

            return randomList;
        }

        private List<Dictionary<string, object>> GridSearch(ICollection<Hyperparameter> hyperparameters)
        {
            if (hyperparameters == null || !hyperparameters.Any())
            {
                return new List<Dictionary<string, object>>();
            }

            // Generuj wszystkie możliwe wartości dla każdego hiperparametru
            var parameterValues = new List<(string Name, List<object> Values)>();

            foreach (var hp in hyperparameters)
            {
                var values = GenerateValueRange(hp.bottomRange, hp.upperRange, hp.step);
                parameterValues.Add((hp.Name, values));
            }

            // Generuj wszystkie kombinacje (iloczyn kartezjański)
            return GenerateCartesianProduct(parameterValues);
        }

        private List<object> GenerateValueRange(decimal min, decimal max, decimal step)
        {
            var values = new List<object>();

            for (decimal value = min; value <= max; value += step)
            {
                values.Add(value); // Możesz tu dodać logikę dla innych typów
            }

            return values;
        }

        private List<Dictionary<string, object>> GenerateCartesianProduct(
            List<(string Name, List<object> Values)> parameterValues)
        {
            var result = new List<Dictionary<string, object>>();

            // Rekurencyjnie generuj wszystkie kombinacje
            GenerateCombinations(parameterValues, 0, new Dictionary<string, object>(), result);

            return result;
        }

        private void GenerateCombinations(
            List<(string Name, List<object> Values)> parameterValues,
            int currentIndex,
            Dictionary<string, object> currentCombination,
            List<Dictionary<string, object>> result)
        {
            if (currentIndex == parameterValues.Count)
            {
                // Dodaj kopię aktualnej kombinacji do wyników
                result.Add(new Dictionary<string, object>(currentCombination));
                return;
            }

            var (name, values) = parameterValues[currentIndex];

            foreach (var value in values)
            {
                currentCombination[name] = value;
                GenerateCombinations(parameterValues, currentIndex + 1, currentCombination, result);
            }
        }
    }
}