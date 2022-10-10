using Domain;
using Domain.Configuration;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Business.Engines
{
	public class SetupGenerationEngine : ISetupGenerationEngine
	{
		private ILogger<SetupGenerationEngine> _logger;

		public SetupGenerationEngine(
			ILogger<SetupGenerationEngine> logger
		)
		{
			_logger = logger;
		}

		public Task<OperationResult> GenerateSetupAsync(OpenAPIData openAPIData, GenerationContext context, Template template)
		{
			throw new NotImplementedException();
		}
	}
}