using Business.Extensions;
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
						AddParameters(verb, jsonParameters, context, document);
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

		private static (Model Model, JsonElement Properties, IEnumerable<string> RequiredProperties) GetModelInfo(JsonProperty component)
		{
			var model = new Model();
			model.Name = NameFactory.Create(component.Name);
			model.TypeName = model.Name.Safe.PascalCase;
			try
			{
				var modelProps = component.Value.EnumerateObject();
				var properties = modelProps.FirstOrDefault(a => a.Name.Equals("properties", Comparison)).Value;
				var requiredPropertiesValue = modelProps.FirstOrDefault(a => a.Name.Equals("required", Comparison));
				var requiredProperties = requiredPropertiesValue.Value.ValueKind == JsonValueKind.Undefined
					? new List<string>()
					: requiredPropertiesValue.Value.EnumerateArray().Select(a => a.ToString());
				return (model, properties, requiredProperties);
			}
			catch (Exception ex)
			{
				throw new Exception($"Error GettingModelInfo '{model.Name.Value}': {ex.Message}");
			}
		}

		private void AddParameters(Verb verb, JsonProperty jsonParameters, GenerationContext context, JsonDocument document)
		{
			// route or querystring parameters
			foreach (var jsonParameter in jsonParameters.Value.EnumerateArray())
			{
				var parameter = jsonParameter.EnumerateObject();
				var property = new Property();
				var parameterName = parameter.First(a => a.Name.Equals("name", Comparison)).Value.ToString();
				property.Name = NameFactory.Create(parameterName);
				var type = jsonParameter.Get("type");
				if (type != null)
				{
					var openApiType = GetOpenApiType(jsonParameter, type.Value.ToString(), document);
					property.Type = TypeFactory.Create(openApiType, context.TypeConfiguration);
					property.IsNullable = openApiType.Nullable;

					var _in = jsonParameter.Get("in").Value.ToString();
					if (_in == "path")
					{
						verb.PathParameters.Add(property);
					}
					else
					{
						verb.Parameters.Add(property);
					}
				}
				else
				{
					var schema = jsonParameter.Get("schema");
					if (schema != null && schema.Value.ValueKind != JsonValueKind.Undefined)
					{
						var propertyProperties = schema.Value.EnumerateObject();
						var openApiType = GetOpenApiType(propertyProperties, document);
						property.Type = TypeFactory.Create(openApiType, context.TypeConfiguration);
						property.IsNullable = openApiType.Nullable;
						verb.Parameters.Add(property);
					}
				}
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
			var twoHundredContentResponse = responses.Value.EnumerateObject()
										.FirstOrDefault(a => a.Name.StartsWith("20", Comparison));
			if (twoHundredContentResponse.Value.ValueKind == JsonValueKind.Undefined)
			{
				return;
			}
			var responseObject = twoHundredContentResponse
						.Value.EnumerateObject()
						.FirstOrDefault(a => a.Name.Equals("schema", Comparison));
			if (responseObject.Value.ValueKind == JsonValueKind.Undefined)
			{
				return;
			}
			var reference = responseObject.Value.Get("$ref");
			if (reference.HasValue)
			{
				var responseObjectName = reference.Value.ToString().Replace("#/definitions/", "");
				verb.ResponseObject = models.FirstOrDefault(a => a.Name.Value.Equals(responseObjectName, Comparison));
			}
		}

		private List<Model> GetModels(JsonDocument document, GenerationContext context)
		{
			var result = new List<Model>();
			var definitionsNode = document.RootElement.EnumerateObject()
								.FirstOrDefault(a => a.Name.Equals("definitions", Comparison));
			if (definitionsNode.Value.ValueKind == JsonValueKind.Undefined)
			{
				return result;
			}
			foreach (var component in definitionsNode.Value.EnumerateObject())
			{
				var modelInfo = GetModelInfo(component);
				if (modelInfo.Properties.ValueKind == JsonValueKind.Undefined)
				{
					modelInfo.Model.TypeName = "dynamic";
				}
				else
				{
					foreach (var jsonProperty in modelInfo.Properties.EnumerateObject())
					{
						var property = new Property();
						property.Name = NameFactory.Create(jsonProperty.Name);
						property.HasSameNameAsType = modelInfo.Model.Name.Value.Equals(property.Name.Value, Comparison);
						var propertyProperties = jsonProperty.Value.EnumerateObject();
						property.IsNullable = modelInfo.RequiredProperties.Any(a => string.Equals(modelInfo.Model.Name, StringComparison.InvariantCultureIgnoreCase));
						if (jsonProperty.Value.ValueKind == JsonValueKind.Object && propertyProperties.Count() == 0)
						{
							property.Type = "dynamic";
							modelInfo.Model.Properties.Add(property);
						}
						else
						{
							var openApiType = GetOpenApiType(propertyProperties, document);
							if (openApiType.Type == "array")
							{
								var x = 1;
							}
							property.Type = TypeFactory.Create(openApiType, context.TypeConfiguration);
							modelInfo.Model.Properties.Add(property);
						}
					}
				}
				result.Add(modelInfo.Model);
			}
			return result;
		}

		private OpenApiType GetOpenApiType(JsonElement jsonParameter, string type, JsonDocument document)
		{
			var typeFormat = string.Empty;
			var nullable = true;
			if (type == "array")
			{
				type = "List<dynamic>";
			}
			// this won't explode if it is null
			if (type.Contains("#/definitions/"))
			{
				type = type.Replace("#/definitions/", "");
				typeFormat = "object";
			}
			else
			{
				typeFormat = jsonParameter.Get("format").ToString();
				nullable = jsonParameter.Get("required").ToString().Equals("true", Comparison) ? false : true;
			}
			return new OpenApiType
			{
				Type = string.IsNullOrWhiteSpace(type) ? "dynamic" : type,
				Format = typeFormat,
				Nullable = nullable
			};
		}

		private OpenApiType GetOpenApiType(JsonElement.ObjectEnumerator propertyProperties, JsonDocument document)
		{
			try
			{
				var reference = propertyProperties.FirstOrDefault(a => a.Name.Equals("$ref", Comparison));
				var type = reference.Value.ValueKind == JsonValueKind.Undefined
					? propertyProperties.FirstOrDefault(a => a.Name.Equals("type", Comparison)).Value.ToString()
					: reference.Value.ToString();
				var typeFormat = string.Empty;
				var nullable = "true";
				if (type == "array")
				{
					type = "List<dynamic>";
				}
				// this won't explode if it is null
				if (type.Contains("#/definitions/"))
				{
					type = type.Replace("#/definitions/", "");
					typeFormat = NameFactory.Create(type).Safe.PascalCase;
					//var x = document.RootElement.Get("definitions")?.Get(type)?.Get("properties").Value.EnumerateObject();
					//propertyProperties = x.Value;
				}
				else
				{
					typeFormat = propertyProperties.FirstOrDefault(a => a.Name.Equals("format", Comparison)).Value.ToString();
					nullable = propertyProperties.FirstOrDefault(a => a.Name.Equals("nullable", Comparison)).Value.ToString();
				}
				return new OpenApiType
				{
					Type = string.IsNullOrWhiteSpace(type) ? "dynamic" : type,
					Format = typeFormat,
					Nullable = string.IsNullOrWhiteSpace(nullable)
						? false
						: BooleanConverter.ReferenceEquals(nullable, false)
				};
			}
			catch (Exception ex)
			{
				throw;
			}
		}
	}
}