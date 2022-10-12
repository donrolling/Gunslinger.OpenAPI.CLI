using Business.Constants;
using Business.Engines;
using Business.Factories;
using Domain;
using Domain.Configuration;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using HandlebarsDotNet;
using Microsoft.Extensions.Logging;

namespace Business.Managers
{
	public class GenerationManager : IGenerationManager
	{
		public GenerationContext Context { get; private set; }

		private readonly IContextFactory _contextFactory;
		private readonly IFileCreationEngine _fileCreationEngine;
		private readonly ILogger<OpenApiParsingEngine> _logger;
		private readonly IOpenApiParsingEngine _openApiParsingEngine;
		private readonly IRenderEngine _renderEngine;

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

		private void AddImportsAndNamespacesToResultModels(OpenApiResult openApiResult, Template template)
		{
			foreach (var model in openApiResult.Models)
			{
				model.Namespace = template.Namespace;
				model.Imports = template.Imports;
			}
			foreach (var path in openApiResult.Routes)
			{
				path.Namespace = template.Namespace;
				path.Imports = template.Imports;
			}
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
					var setupGenerationResult = await GenerateSetupAsync(Context, template, openApiResult);
					if (setupGenerationResult.Failed)
					{
						errors.Add(setupGenerationResult);
					}
					break;
			}
		}

		private async Task<OperationResult> GenerateManyAsync(GenerationContext context, Template template, OpenApiResult openApiResult)
		{
			AddImportsAndNamespacesToResultModels(openApiResult, template);
			switch (template.Type)
			{
				case TemplateType.Path:
					return await GeneratePathsAsync(context, template, openApiResult);

				case TemplateType.Model:
					return await GenerateModelsAsync(context, template, openApiResult);

				case TemplateType.Setup:
				default:
					return OperationResult.Fail($"Type: {template.Type} should not have been in the GenerateManyAsync method.");
			}
		}

		private async Task<OperationResult> GenerateModelsAsync(GenerationContext context, Template template, OpenApiResult openApiResult)
		{
			var errors = new List<string>();
			foreach (var model in openApiResult.Models)
			{
				var prepareDirectoryResult = PrepareDirectory(context, template, model);
				if (prepareDirectoryResult.Failed)
				{
					return prepareDirectoryResult;
				}
				var path = prepareDirectoryResult.Result;
				// don't output the excluded types
				if (template.ExcludeTheseTypes.Contains(model.Name.Value))
				{
					continue;
				}
				try
				{
					var outputResult = _renderEngine.Render(template.TemplateText, model);
					if (outputResult.Failed)
					{
						return outputResult;
					}
					var result = await _fileCreationEngine.CreateFileAsync(path, outputResult.Result);
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

		private async Task<OperationResult> GeneratePathsAsync(GenerationContext context, Template template, OpenApiResult openApiResult)
		{
			var errors = new List<string>();
			foreach (var path in openApiResult.Routes)
			{
				var prepareDirectoryResult = PrepareDirectory(context, template, path);
				if (prepareDirectoryResult.Failed)
				{
					return prepareDirectoryResult;
				}
				var outputPath = prepareDirectoryResult.Result;
				// don't output the excluded types
				if (template.ExcludeTheseTypes.Contains(path.Name.Value))
				{
					continue;
				}
				try
				{
					var outputResult = _renderEngine.Render(template.TemplateText, path);
					if (outputResult.Failed)
					{
						return outputResult;
					}
					var result = await _fileCreationEngine.CreateFileAsync(outputPath, outputResult.Result);
					if (result.Failed)
					{
						var message = $"EntityName: {path.Name.Value}\r\nTemplateName: {template.Name} - {result.Message}";
						errors.Add(message);
					}
				}
				catch (Exception ex)
				{
					var message = $"EntityName: {path.Name.Value}\r\nTemplateName: {template.Name} - {ex.Message}";
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

		private async Task<OperationResult> GenerateSetupAsync(GenerationContext context, Template template, OpenApiResult openApiResult)
		{
			var prepareDirectoryResult = PrepareDirectory(context, template);
			if (prepareDirectoryResult.Failed)
			{
				return prepareDirectoryResult;
			}
			var path = prepareDirectoryResult.Result;
			var modelGroup = new ModelGroup
			{
				Name = NameFactory.Create(template.Name),
				Data = openApiResult,
				Namespace = template.Namespace,
				Imports = template.Imports
			};
			var outputResult = _renderEngine.Render(template.TemplateText, modelGroup);
			if (outputResult.Failed)
			{
				return outputResult;
			}
			return await _fileCreationEngine.CreateFileAsync(path, outputResult.Result);
		}

		private OperationResult<string> PrepareDirectory(GenerationContext context, Template template, INamed item)
		{
			var outputRelativePath = template.OutputRelativePath;
			var root = string.IsNullOrWhiteSpace(context.OutputDirectory) ? context.RootPath: context.OutputDirectory;
			var path = $"{root}\\{outputRelativePath}".Replace(ConfigTemplateStrings.EntityName, item.Name.Value);
			var prepareDirectoryResult = _fileCreationEngine.PrepareOutputDirectory(path, template.DeleteAllItemsInOutputDirectory);
			return prepareDirectoryResult.Success ? OperationResult.Ok(path) : OperationResult.Fail<string>(prepareDirectoryResult.Message);
		}

		private OperationResult<string> PrepareDirectory(GenerationContext context, Template template)
		{
			var outputRelativePath = template.OutputRelativePath;
			var root = string.IsNullOrWhiteSpace(context.OutputDirectory) ? context.RootPath : context.OutputDirectory;
			var path = $"{root}\\{outputRelativePath}";
			var prepareDirectoryResult = _fileCreationEngine.PrepareOutputDirectory(path, template.DeleteAllItemsInOutputDirectory);
			return prepareDirectoryResult.Success ? OperationResult.Ok(path) : OperationResult.Fail<string>(prepareDirectoryResult.Message);
		}
	}
}