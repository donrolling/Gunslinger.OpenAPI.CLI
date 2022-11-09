using Domain;
using Domain.Configuration;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using System.Text.Json;
using Utilities.IO;

namespace Business.Engines
{
	public class JsonDocumentEngine : IJsonDocumentEngine
	{
		private static readonly Dictionary<string, OpenAPIData> _openAPIData;
		private readonly IHttpClientFactory _httpClientFactory;

		static JsonDocumentEngine()
		{
			_openAPIData = new Dictionary<string, OpenAPIData>();
		}

		public JsonDocumentEngine(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}

		public async Task<OperationResult<ApiDoc>> GetJsonDocAsync(GenerationContext context, string dataProviderName)
		{
			var openAPIData = await GetOpenAPISettingsAsync(context, dataProviderName);
			if (openAPIData.Failed)
			{
				return OperationResult.Fail<ApiDoc>(openAPIData.Message);
			}
			var options = new JsonDocumentOptions
			{
				CommentHandling = JsonCommentHandling.Skip
			};
			var result = new ApiDoc();
			result.JsonDocument = JsonDocument.Parse(openAPIData.Result.Data, options);
			result.SupportedDocTypes = GetDocType(result.JsonDocument);
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

		private SupportedDocTypes GetDocType(JsonDocument document)
		{
			var swaggerDocTypeProperty = document.RootElement.EnumerateObject().FirstOrDefault(a => a.NameEquals("swagger"));
			var swaggerVal = swaggerDocTypeProperty.Value.ToString();
			if (!string.IsNullOrEmpty(swaggerVal) && swaggerVal == "2.0")
			{
				return SupportedDocTypes.Swagger_2_0;
			}
			var openApiDocTypeProperty = document.RootElement.EnumerateObject().FirstOrDefault(a => a.NameEquals("openapi"));
			var openApiVal = openApiDocTypeProperty.Value.ToString();
			//"openapi": "3.0.1",
			if (!string.IsNullOrEmpty(openApiVal) && openApiVal.StartsWith("3.0"))
			{
				return SupportedDocTypes.OpenAPI_3_0;
			}
			return SupportedDocTypes.Unknown;
		}

		private async Task<OperationResult<OpenAPIData>> GetOpenAPISettingsAsync(GenerationContext context, string dataProviderName)
		{
			var dataProvider = context
					.DataProviders
					.FirstOrDefault(a =>
						string.Equals(a.Name, dataProviderName, StringComparison.InvariantCultureIgnoreCase)
					);
			if (dataProvider == null)
			{
				return OperationResult.Fail<OpenAPIData>($"Could not find a data provider named: {dataProviderName}");
			}
			if (string.IsNullOrWhiteSpace(dataProvider.Location))
			{
				return OperationResult.Fail<OpenAPIData>($"Data provider named: {dataProviderName} had a missing Location parameter.");
			}
			if (_openAPIData.ContainsKey(dataProvider.Name))
			{
				OpenAPIData value = null;
				if (!_openAPIData.TryGetValue(dataProvider.Name, out value))
				{
					return OperationResult.Fail<OpenAPIData>($"Attempted to retrieve data provider named: {dataProviderName}, but failed.");
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
	}
}