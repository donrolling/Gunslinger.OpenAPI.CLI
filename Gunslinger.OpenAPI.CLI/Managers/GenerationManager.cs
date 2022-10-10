using Domain;
using Domain.Configuration;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using Gunslinger.OpenAPI.CLI.Engines;
using Microsoft.Extensions.Logging;

namespace Gunslinger.OpenAPI.CLI.Managers
{
	public class GenerationManager : IGenerationManager
	{
		public GenerationContext Context { get; private set; }

		private readonly IContextFactory _contextFactory;
		private readonly IGenerationEngine _generationEngine;
		private readonly ILogger<GenerationEngine> _logger;

		public GenerationManager(
			IContextFactory contextFactory,
			IOpenApiDataProvider openApiDataProvider,
			IGenerationEngine generationEngine,
			ILogger<GenerationEngine> logger
		)
		{
			_contextFactory = contextFactory;
			_generationEngine = generationEngine;
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

			foreach (var template in Context.Templates)
			{
				await GenerateFiles(template, errors);
			}

			// done
			if (errors.Any())
			{
				var message = errors.Select(a => a.Message).Aggregate("Generation was not completely successful.\r\n\t", (accumulator, next) => $"{accumulator}\r\n\t{next}");
				return OperationResult.Fail(message);
			}
			return OperationResult.Ok();
		}

		private async Task GenerateFiles(Template template, List<OperationResult> errors)
		{
			switch (template.Type)
			{
				case TemplateType.Model:
					var modelGenerationResult = await _generationEngine.GenerateModels(Context, template);
					if (modelGenerationResult.Failed)
					{
						errors.Add(modelGenerationResult);
					}
					break;

				case TemplateType.Path:
					var pathGenerationResult = await _generationEngine.GeneratePaths(Context, template);
					if (pathGenerationResult.Failed)
					{
						errors.Add(pathGenerationResult);
					}
					break;

				case TemplateType.Setup:
				default:
					var setupGenerationResult = await _generationEngine.GenerateSetup(Context, template);
					if (setupGenerationResult.Failed)
					{
						errors.Add(setupGenerationResult);
					}
					break;
			}
		}
	}
}