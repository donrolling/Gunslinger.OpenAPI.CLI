using Domain;
using Domain.Configuration;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Business.Engines
{
	public class ModelGenerationEngine : IModelGenerationEngine
	{
		private ILogger<ModelGenerationEngine> _logger;

		public ModelGenerationEngine(
			ILogger<ModelGenerationEngine> logger
		)
		{
			_logger = logger;
		}

		public Task<OperationResult> GenerateModelsAsync(OpenAPIData openAPIData, GenerationContext context, Template template)
		{
			throw new NotImplementedException();
		}
	}
}