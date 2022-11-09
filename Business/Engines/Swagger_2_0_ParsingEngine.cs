using Business.Factories;
using Domain;
using Domain.Configuration;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Text.Json;
using Route = Domain.Models.Route;

namespace Business.Engines
{
	public class Swagger_2_0_ParsingEngine : IParsingEngine
	{
		public SupportedDocTypes SupportedDocTypes => SupportedDocTypes.Swagger_2_0;

		private const StringComparison Comparison = StringComparison.InvariantCultureIgnoreCase;
		
		private ILogger<OpenApiParsingEngine> _logger;

		public Swagger_2_0_ParsingEngine(
			ILogger<OpenApiParsingEngine> logger
		)
		{
			_logger = logger;
		}

		public List<Route> GetPaths(JsonDocument document, List<Model> models, GenerationContext context)
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
						AddParameters(verb, jsonParameters, context);
					}
					AddResponseObjects(jsonVerb, verb, models);
					route.Verbs.Add(verb);
				}
				result.Add(route);
			}
			return result;
		}

		public OperationResult<OpenApiResult> Parse(JsonDocument document, GenerationContext context)
		{
			var models = GetModels(document, context);
			var paths = GetPaths(document, models, context);
			return OperationResult.Ok(new OpenApiResult { Models = models, Routes = paths });
		}

		private void AddParameters(Verb verb, JsonProperty jsonParameters, GenerationContext context)
		{
			// route or querystring parameters
			foreach (var jsonParameter in jsonParameters.Value.EnumerateArray())
			{
				var parameter = jsonParameter.EnumerateObject();
				var property = new Property();
				var parameterName = parameter.First(a => a.Name.Equals("name", Comparison)).Value.ToString();
				property.Name = NameFactory.Create(parameterName);
				var propertyProperties = parameter.First(a => a.Name.Equals("schema", Comparison)).Value.EnumerateObject();
				var openApiType = GetOpenApiType(propertyProperties);
				property.Type = TypeFactory.Create(openApiType, context.TypeConfiguration);
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

		private List<Model> GetModels(JsonDocument document, GenerationContext context)
		{
			var result = new List<Model>();
			var componentsNode = document.RootElement.EnumerateObject()
								.FirstOrDefault(a => a.Name.Equals("components", Comparison));
			if (componentsNode.Value.ValueKind == JsonValueKind.Undefined)
			{
				return result;
			}
			var schemaNode = componentsNode
								.Value.EnumerateObject()
								.FirstOrDefault(a => a.Name.Equals("schemas", Comparison));
			if (schemaNode.Value.ValueKind == JsonValueKind.Undefined)
			{
				return result;
			}
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
					var openApiType = GetOpenApiType(propertyProperties);
					property.Type = TypeFactory.Create(openApiType, context.TypeConfiguration);
					var isNullable = propertyProperties.FirstOrDefault(a => a.Name.Equals("nullable", Comparison)).Value.ToString();
					property.IsNullable = isNullable.Equals("true", Comparison) ? true : false;
					model.Properties.Add(property);
				}
				result.Add(model);
			}
			return result;
		}

		private OpenApiType GetOpenApiType(JsonElement.ObjectEnumerator propertyProperties)
		{
			var type = propertyProperties.First(a => a.Name.Equals("type", Comparison)).Value.ToString();
			// this won't explode if it is null
			var typeFormat = propertyProperties.FirstOrDefault(a => a.Name.Equals("format", Comparison)).Value.ToString();
			var nullable = propertyProperties.FirstOrDefault(a => a.Name.Equals("nullable", Comparison)).Value.ToString();

			return new OpenApiType
			{
				Type = type,
				Format = typeFormat,
				Nullable = string.IsNullOrWhiteSpace(nullable)
					? false
					: BooleanConverter.ReferenceEquals(nullable, false)
			};
		}
	}
}