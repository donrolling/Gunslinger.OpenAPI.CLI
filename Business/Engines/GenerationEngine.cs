using Domain;
using Domain.Configuration;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Utilities.IO;

namespace Business.Engines
{
	public class GenerationEngine : IGenerationEngine
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IModelGenerationEngine _modelGenerationEngine;
		private readonly IPathGenerationEngine _pathGenerationEngine;
		private readonly ISetupGenerationEngine _setupGenerationEngine;
		private ILogger<GenerationEngine> _logger;

		private static readonly Dictionary<string, OpenAPIData> _openAPIData;

		static GenerationEngine()
		{
			_openAPIData = new Dictionary<string, OpenAPIData>();
		}

		public GenerationEngine(
			IHttpClientFactory httpClientFactory,
			IModelGenerationEngine modelGenerationEngine,
			IPathGenerationEngine pathGenerationEngine,
			ISetupGenerationEngine setupGenerationEngine,
			ILogger<GenerationEngine> logger
		)
		{
			_httpClientFactory = httpClientFactory;
			_modelGenerationEngine = modelGenerationEngine;
			_pathGenerationEngine = pathGenerationEngine;
			_setupGenerationEngine = setupGenerationEngine;
			_logger = logger;
		}

		public async Task<OperationResult> GenerateModelsAsync(GenerationContext context, Template template)
		{
			var openAPIData = await GetOpenAPISettingsAsync(context, template);
			if (openAPIData.Failed)
			{
				return openAPIData;
			}
			return await _modelGenerationEngine.GenerateModelsAsync(openAPIData.Result, context, template);
		}

		public async Task<OperationResult> GeneratePathsAsync(GenerationContext context, Template template)
		{
			var openAPIData = await GetOpenAPISettingsAsync(context, template);
			if (openAPIData.Failed)
			{
				return openAPIData;
			}
			return await _pathGenerationEngine.GeneratePathsAsync(openAPIData.Result, context, template);
		}

		public async Task<OperationResult> GenerateSetupAsync(GenerationContext context, Template template)
		{
			var openAPIData = await GetOpenAPISettingsAsync(context, template);
			if (openAPIData.Failed)
			{
				return openAPIData;
			}
			return await _setupGenerationEngine.GenerateSetupAsync(openAPIData.Result, context, template);
		}

		private async Task<OperationResult<OpenAPIData>> GetOpenAPISettingsAsync(GenerationContext context, Template template)
		{
			var dataProvider = context
					.DataProviders
					.FirstOrDefault(a =>
						string.Equals(a.Name, template.DataProviderName, StringComparison.InvariantCultureIgnoreCase)
					);
			if (dataProvider == null)
			{
				return OperationResult.Fail<OpenAPIData>($"Could not find a data provider named: {template.DataProviderName}");
			}
			if (string.IsNullOrWhiteSpace(dataProvider.Location))
			{
				return OperationResult.Fail<OpenAPIData>($"Data provider named: {template.DataProviderName} had a missing Location parameter.");
			}
			if (_openAPIData.ContainsKey(dataProvider.Name))
			{
				OpenAPIData value = null;
				if (!_openAPIData.TryGetValue(dataProvider.Name, out value))
				{
					return OperationResult.Fail<OpenAPIData>($"Attempted to retrieve data provider named: {template.DataProviderName}, but failed.");
				}
				return OperationResult.Ok(value);
			}
			var dataResult = await GetDataAsync(context, dataProvider);
			if (dataResult.Failed)
			{
				return OperationResult.Fail<OpenAPIData>(dataResult.Message);
			}
			var result = new OpenAPIData { Name = dataProvider.Name, Location = dataProvider.Location, Data = dataResult.Result };
			_openAPIData.Add(dataProvider.Name, result);
			return OperationResult.Ok(result);
		}

		private async Task<OperationResult<string>> GetDataAsync(GenerationContext context, OpenAPISettings dataProvider)
		{
			// datasource could be an url or a path
			var isWellFormedUriString = Uri.IsWellFormedUriString(dataProvider.Location, UriKind.Absolute);
			if (isWellFormedUriString)
			{
				return await GetDataFromUrlAsync(dataProvider);
			}
			else
			{
				return await GetDataFromPathAsync(context, dataProvider);
			}
		}

		private async Task<OperationResult<string>> GetDataFromPathAsync(GenerationContext context, OpenAPISettings dataProvider)
		{
			// give us a full path if there isn't one already
			if (!dataProvider.Location.Contains(":\\"))
			{
				dataProvider.Location = $"{context.RootPath}\\{dataProvider.Location}";
			}
			var result = await FileUtility.ReadAllTextAsync(dataProvider.Location);
			return OperationResult.Ok(result);
		}

		private async Task<OperationResult<string>> GetDataFromUrlAsync(OpenAPISettings dataProvider)
		{
			var response = await _httpClientFactory.CreateClient().GetAsync(dataProvider.Location);
			if (!response.IsSuccessStatusCode)
			{
				return OperationResult.Fail<string>($"Http call failed '{dataProvider.Location}'");
			}
			var content = await response.Content.ReadAsStringAsync();
			if (string.IsNullOrWhiteSpace(content))
			{
				return OperationResult.Fail<string>($"Http call returned no data");
			}
			return OperationResult.Ok(content);
		}
	}
}