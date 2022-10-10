using Domain;
using Domain.Configuration;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Gunslinger.OpenAPI.CLI.Engines
{
	public class GenerationEngine : IGenerationEngine
	{
		private readonly IOpenApiDataProvider _openApiDataProvider;
		private ILogger<GenerationEngine> _logger;

		public GenerationEngine(
			IOpenApiDataProvider openApiDataProvider,
			ILogger<GenerationEngine> logger
		)
		{
			_openApiDataProvider = openApiDataProvider;
			_logger = logger;
		}

		public async Task<OperationResult> GenerateModels(GenerationContext context, Template template)
		{
			var openAPISettings = GetOpenAPISettings(context, template);
			if (openAPISettings == null)
			{
				return OperationResult.Fail($"Could not find settings: {template.DataProviderName}");
			}
			throw new NotImplementedException();
		}

		public async Task<OperationResult> GeneratePaths(GenerationContext context, Template template)
		{
			var openAPISettings = GetOpenAPISettings(context, template);
			if (openAPISettings == null)
			{
				return OperationResult.Fail($"Could not find settings: {template.DataProviderName}");
			}
			throw new NotImplementedException();
		}

		public async Task<OperationResult> GenerateSetup(GenerationContext context, Template template)
		{
			var openAPISettings = GetOpenAPISettings(context, template);
			if (openAPISettings == null)
			{
				return OperationResult.Fail($"Could not find settings: {template.DataProviderName}");
			}
			throw new NotImplementedException();
		}

		private OpenAPISettings GetOpenAPISettings(GenerationContext context, Template template)
		{
			return context.DataProviders.FirstOrDefault(a => string.Equals(a.Name, template.DataProviderName, StringComparison.InvariantCultureIgnoreCase));
		}
	}
}