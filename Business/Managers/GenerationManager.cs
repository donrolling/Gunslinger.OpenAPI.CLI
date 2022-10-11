using Business.Engines;
using Business.Factories;
using Domain;
using Domain.Configuration;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using HandlebarsDotNet;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Path = System.IO.Path;

namespace Business.Managers
{
	public class GenerationManager : IGenerationManager
	{
		public GenerationContext Context { get; private set; }

		private readonly IContextFactory _contextFactory;
		private readonly IFileCreationEngine _fileCreationEngine;
		private readonly IOpenApiParsingEngine _openApiParsingEngine;
		private readonly IRenderEngine _renderEngine;
		private readonly ILogger<OpenApiParsingEngine> _logger;

		public GenerationManager(
			IContextFactory contextFactory,
			IFileCreationEngine fileCreationEngine,
			IOpenApiParsingEngine openApiParsingEngine,
			IRenderEngine renderEngine,
			ILogger<OpenApiParsingEngine> logger
		)
		{
			_contextFactory = contextFactory;
			_openApiParsingEngine = openApiParsingEngine;
			_renderEngine = renderEngine;
			_fileCreationEngine = fileCreationEngine;
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
					await GenerateFilesAsync(template, openApiResult.Result, errors);
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

		private async Task GenerateFilesAsync(Template template, OpenApiResult openApiResult, List<OperationResult> errors)
		{
			switch (template.Type)
			{
				case TemplateType.Model:
				case TemplateType.Path:
					var modelGenerationResult = await GenerateManyAsync(Context, template, openApiResult);
					if (modelGenerationResult.Failed)
					{
						errors.Add(modelGenerationResult);
					}
					break;

				case TemplateType.Setup:
				default:
					var setupGenerationResult = await GenerateOneAsync(Context, template, openApiResult);
					if (setupGenerationResult.Failed)
					{
						errors.Add(setupGenerationResult);
					}
					break;
			}
		}

		private async Task<OperationResult> GenerateManyAsync(GenerationContext context, Template template, OpenApiResult openApiResult)
		{
			var errors = new List<string>();
			foreach (var model in openApiResult.Models)
			{
				var outputRelativePath = template.OutputRelativePath;
				var path = $"{context.RootPath}\\{outputRelativePath}".Replace("{entityName}", model.Name.Value);
				var destinationPath = _fileCreationEngine.PrepareOutputDirectory(path, template.DeleteAllItemsInOutputDirectory);
				// don't output the excluded types
				if (template.ExcludeTheseTypes.Contains(model.Name.Value))
				{
					continue;
				}
				try
				{
					var output = _renderEngine.Render(template.TemplateText, model);
					var result = await _fileCreationEngine.CreateFileAsync(path, output);
					if (result.Failed)
					{
						var message = $"EntityName: {model.Name.Value}\r\nTemplateName: {template.Name} - {result.Message}";
						errors.Add(message);
					}
				}
				catch (Exception ex)
				{
					var message = $"EntityName: {model.Name.Value}\r\nTemplateName: {template.Name} - {ex.Message}";
					errors.Add(message);
				}
			}
			if (errors.Any())
			{
				var errorMessages = string.Join(Environment.NewLine, errors);
				var message = $"Partial or total generation failure: {errorMessages}";
				return OperationResult.Fail(message);
			}
			return OperationResult.Ok();
		}

		private async Task<OperationResult> GenerateOneAsync(GenerationContext context, Template template, OpenApiResult openApiResult)
		{
			var outputRelativePath = template.OutputRelativePath;
			var path = $"{context.RootPath}\\{outputRelativePath}";
			// don't need to perform 'entityName' replacement on single-file generation, because it doesn't make sense.
			var destinationPath = _fileCreationEngine.PrepareOutputDirectory(path, template.DeleteAllItemsInOutputDirectory);			
			var fileName = Path.GetFileName(path);
			var modelGroup = new ModelGroup
			{
				Name = NameFactory.Create(fileName),
				Data = openApiResult,
				Namespace = template.Namespace,
				Imports = template.Imports
			};
			var output = _renderEngine.Render(template.TemplateText, modelGroup);
			return await _fileCreationEngine.CreateFileAsync(path, output);
		}
	}
}