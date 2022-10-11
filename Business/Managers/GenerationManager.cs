using Business.Engines;
using Domain;
using Domain.Configuration;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Business.Managers
{
	public class GenerationManager : IGenerationManager
	{
		public GenerationContext Context { get; private set; }

		private readonly IContextFactory _contextFactory;
		private readonly IOpenApiParsingEngine _openApiParsingEngine;
		private readonly ILogger<OpenApiParsingEngine> _logger;

		public GenerationManager(
			IContextFactory contextFactory,
			IOpenApiParsingEngine generationEngine,
			ILogger<OpenApiParsingEngine> logger
		)
		{
			_contextFactory = contextFactory;
			_openApiParsingEngine = generationEngine;
			_logger = logger;
		}

		public async Task<OperationResult> GenerateAsync(CommandSettings commandSettings)
		{
			var contextResult = _contextFactory.Create(commandSettings);
			if (contextResult.Failed)
			{
				return contextResult;
			}
			Context = contextResult.Result;

			var errors = new List<OperationResult>();
			var dataProviderNames = Context.Templates.Select(a => a.DataProviderName).Distinct();
			foreach (var dataProviderName in dataProviderNames)
			{
				var openApiResult = await _openApiParsingEngine.ParseOpenApiAsync(Context, dataProviderName);
				var templates = Context.Templates.Where(a => a.DataProviderName.Equals(dataProviderName, StringComparison.InvariantCultureIgnoreCase));
				foreach (var template in templates)
				{
					await GenerateFiles(template, openApiResult, errors);
				}
			}

			// done
			if (errors.Any())
			{
				var message = errors.Select(a => a.Message).Aggregate("Generation was not completely successful.\r\n\t", (accumulator, next) => $"{accumulator}\r\n\t{next}");
				return OperationResult.Fail(message);
			}
			return OperationResult.Ok();
		}

		private async Task GenerateFiles(Template template, OperationResult<OpenApiResult> openApiResult, List<OperationResult> errors)
		{
			switch (template.Type)
			{
				case TemplateType.Model:
					var modelGenerationResult =
					if (modelGenerationResult.Failed)
					{
						errors.Add(modelGenerationResult);
					}
					break;

				case TemplateType.Path:
					var pathGenerationResult = await _openApiParsingEngine.GeneratePathsAsync(Context, template);
					if (pathGenerationResult.Failed)
					{
						errors.Add(pathGenerationResult);
					}
					break;

				case TemplateType.Setup:
				default:
					var setupGenerationResult = await _openApiParsingEngine.GenerateSetupAsync(Context, template);
					if (setupGenerationResult.Failed)
					{
						errors.Add(setupGenerationResult);
					}
					break;
			}
		}
	}
}