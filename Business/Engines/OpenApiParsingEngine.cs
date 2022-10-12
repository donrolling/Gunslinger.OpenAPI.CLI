using Business.Factories;
using Domain;
using Domain.Configuration;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Utilities.IO;
using Route = Domain.Models.Route;

namespace Business.Engines
{
	public class OpenApiParsingEngine : IOpenApiParsingEngine
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private ILogger<OpenApiParsingEngine> _logger;

		private static readonly Dictionary<string, OpenAPIData> _openAPIData;
		private const StringComparison Comparison = StringComparison.InvariantCultureIgnoreCase;

		static OpenApiParsingEngine()
		{
			_openAPIData = new Dictionary<string, OpenAPIData>();
		}

		public OpenApiParsingEngine(
			IHttpClientFactory httpClientFactory,
			ILogger<OpenApiParsingEngine> logger
		)
		{
			_httpClientFactory = httpClientFactory;
			_logger = logger;
		}

		public async Task<OperationResult<OpenApiResult>> ParseOpenApiAsync(GenerationContext context, string dataProviderName)
		{
			var openAPIData = await GetOpenAPISettingsAsync(context, dataProviderName);
			if (openAPIData.Failed)
			{
				return OperationResult.Fail<OpenApiResult>(openAPIData.Message);
			}
			var options = new JsonDocumentOptions
			{
				CommentHandling = JsonCommentHandling.Skip
			};
			using (var document = JsonDocument.Parse(openAPIData.Result.Data, options))
			{
				var models = GetModels(document);
				var paths = GetPaths(document, models);
				return OperationResult.Ok(new OpenApiResult { Models = models, Paths = paths });
			}
		}

		private List<Model> GetModels(JsonDocument document)
		{
			var result = new List<Model>();
			var schemaNode = document.RootElement.EnumerateObject()
								.First(a => a.Name.Equals("components", Comparison))
								.Value.EnumerateObject()
								.First(a => a.Name.Equals("schemas", Comparison));
			foreach (var component in schemaNode.Value.EnumerateObject())
			{
				var model = new Model();
				model.Name = NameFactory.Create(component.Name);
				var modelProps = component.Value.EnumerateObject();
				model.TypeName = modelProps.First(a => a.Name.Equals("type", Comparison)).Name;
				var properties = modelProps.First(a => a.Name.Equals("properties", Comparison)).Value;
				foreach (var jsonProperty in properties.EnumerateObject())
				{
					var property = new Property();
					property.Name = NameFactory.Create(jsonProperty.Name);
					var propertyProperties = jsonProperty.Value.EnumerateObject();
					var type = propertyProperties.First(a => a.Name.Equals("type", Comparison)).Value.ToString();
					// this won't explode if it is null
					var typeFormat = propertyProperties.FirstOrDefault(a => a.Name.Equals("format", Comparison)).Value.ToString();
					property.Type = TypeFactory.Create(type, typeFormat);
					var isNullable = propertyProperties.FirstOrDefault(a => a.Name.Equals("nullable", Comparison)).Value.ToString();
					property.IsNullable = isNullable.Equals("true", Comparison) ? true : false;
					model.Properties.Add(property);
				}
				result.Add(model);
			}
			return result;
		}

		public List<Route> GetPaths(JsonDocument document, List<Model> models)
		{
			var result = new List<Route>();
			var pathsNode = document.RootElement.EnumerateObject()
								.First(a => a.Name.Equals("paths", Comparison));
			foreach (var pathNode in pathsNode.Value.EnumerateObject())
			{
				var route = new Route();
				var pathName = pathNode.Name.Replace("/api/", "").Replace("/", "_");
				route.Name = NameFactory.Create(pathName);
				route.Path = pathNode.Name;
				var jsonVerbs = pathNode.Value.EnumerateObject();
				foreach (var jsonVerb in jsonVerbs)
				{
					var verb = new Verb();
					verb.Name = NameFactory.Create(jsonVerb.Name);
					var jsonParameters = jsonVerb.Value.EnumerateObject().FirstOrDefault(a => a.Name.Equals("parameters", Comparison));
					if (jsonParameters.Value.ValueKind == JsonValueKind.Undefined)
					{
						AddRequestObjects(jsonVerb, verb, models);
					}
					else
					{
						AddParameters(verb, jsonParameters);
					}
					AddResponseObjects(jsonVerb, verb, models);
					route.Verbs.Add(verb);
				}
				result.Add(route);
			}
			return result;
		}

		private static void AddParameters(Verb verb, JsonProperty jsonParameters)
		{
			// route or querystring parameters
			foreach (var jsonParameter in jsonParameters.Value.EnumerateArray())
			{
				var parameter = jsonParameter.EnumerateObject();
				var property = new Property();
				var parameterName = parameter.First(a => a.Name.Equals("name", Comparison)).Value.ToString();
				property.Name = NameFactory.Create(parameterName);
				var propertyProperties = parameter.First(a => a.Name.Equals("schema", Comparison)).Value.EnumerateObject();
				var type = propertyProperties.First(a => a.Name.Equals("type", Comparison)).Value.ToString();
				// this won't explode if it is null
				var typeFormat = propertyProperties.FirstOrDefault(a => a.Name.Equals("format", Comparison)).Value.ToString();
				property.Type = property.Type = TypeFactory.Create(type, typeFormat);
				var isNullable = propertyProperties.FirstOrDefault(a => a.Name.Equals("nullable", Comparison)).Value.ToString();
				property.IsNullable = isNullable.Equals("true", Comparison) ? true : false;
				verb.Parameters.Add(property);
			}
		}

		private void AddRequestObjects(JsonProperty jsonVerb, Verb verb, List<Model> models)
		{
			var requestBody = jsonVerb.Value.EnumerateObject().FirstOrDefault(a => a.Name.Equals("requestBody", Comparison));
			if (requestBody.Value.ValueKind == JsonValueKind.Undefined)
			{
				return;
			}
			var requestObject = requestBody.Value.EnumerateObject()
						.First(a => a.Name.Equals("content", Comparison))
						.Value.EnumerateObject()
						.FirstOrDefault(a => a.Name.Equals("application/json", Comparison))
						.Value.EnumerateObject()
						.FirstOrDefault(a => a.Name.Equals("schema", Comparison))
						.Value.EnumerateObject()
						.FirstOrDefault(a => a.Name.Equals("$ref", Comparison))
						.Value.ToString();
			if (!string.IsNullOrEmpty(requestObject))
			{
				var requestObjectName = requestObject.Split('/').Last();
				verb.RequestObject = models.FirstOrDefault(a => a.Name.Value.Equals(requestObjectName, Comparison));
			}
		}

		private void AddResponseObjects(JsonProperty jsonVerb, Verb verb, List<Model> models)
		{
			var responses = jsonVerb.Value.EnumerateObject().FirstOrDefault(a => a.Name.Equals("responses", Comparison));
			if (responses.Value.ValueKind == JsonValueKind.Undefined)
			{
				return;
			}
			var twoHundredContentResponse = responses
										.Value.EnumerateObject()
										.FirstOrDefault(a => a.Name.StartsWith("20", Comparison)).Value.EnumerateObject()
										.FirstOrDefault(a => a.Name.Equals("content", Comparison));
			if (twoHundredContentResponse.Value.ValueKind == JsonValueKind.Undefined)
			{
				return;
			}
			var responseObject = twoHundredContentResponse
						.Value.EnumerateObject()
						.FirstOrDefault(a => a.Name.Equals("application/json", Comparison))
						.Value.EnumerateObject()
						.FirstOrDefault(a => a.Name.Equals("schema", Comparison))
						.Value.EnumerateObject()
						.FirstOrDefault(a => a.Name.Equals("$ref", Comparison));
			if (responseObject.Value.ValueKind == JsonValueKind.Undefined)
			{
				return;
			}
			var responseObjectString = responseObject.Value.ToString();
			if (!string.IsNullOrEmpty(responseObjectString))
			{
				var responseObjectName = responseObjectString.Split('/').Last();
				verb.ResponseObject = models.FirstOrDefault(a => a.Name.Value.Equals(responseObjectName, Comparison));
			}
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