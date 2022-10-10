using Domain;
using Domain.Configuration;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Business.Engines
{
	public class PathGenerationEngine : IPathGenerationEngine
	{
		private ILogger<PathGenerationEngine> _logger;

		public PathGenerationEngine(
			ILogger<PathGenerationEngine> logger
		)
		{
			_logger = logger;
		}

		public Task<OperationResult> GeneratePathsAsync(OpenAPIData openAPIData, GenerationContext context, Template template)
		{
			throw new NotImplementedException();
		}
	}
}