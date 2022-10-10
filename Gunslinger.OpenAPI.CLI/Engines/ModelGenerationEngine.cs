using Domain;
using Domain.Configuration;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Gunslinger.OpenAPI.CLI.Engines
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

		public async Task<OperationResult> GenerateModels(GenerationContext context, Template template)
		{
			return null;
		}

	}
}