﻿using Business.Factories;
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
	public class OpenApiParsingEngine : IParsingEngine
	{
		public SupportedDocTypes SupportedDocTypes => SupportedDocTypes.OpenAPI_3_0;

		private const StringComparison Comparison = StringComparison.InvariantCultureIgnoreCase;

		private readonly ILogger<OpenApiParsingEngine> _logger;

		public OpenApiParsingEngine(
			ILogger<OpenApiParsingEngine> logger
		)
		{
			_logger = logger;
		}

		public OperationResult<OpenApiResult> Parse(JsonDocument document, GenerationContext context)
		{
			var models = GetModels(document, context);
			var paths = GetPaths(document, models, context);
			return OperationResult.Ok(new OpenApiResult { Models = models, Routes = paths });
		}

		private static void AddParameters(Verb verb, JsonProperty jsonParameters, GenerationContext context, List<Model> models)
		{
			// route or querystring parameters
			foreach (var jsonParameter in jsonParameters.Value.EnumerateArray())
			{
				var parameter = jsonParameter.EnumerateObject();
				var property = new Property();
				var parameterName = parameter.First(a => a.Name.Equals("name", Comparison)).Value.ToString();
				property.Name = NameFactory.Create(parameterName);
				var propertyProperties = parameter.First(a => a.Name.Equals("schema", Comparison)).Value.EnumerateObject();
				var openApiType = GetOpenApiType(propertyProperties, models);
				property.Type = TypeFactory.Create(openApiType, context.TypeConfiguration);
				var isNullable = propertyProperties.FirstOrDefault(a => a.Name.Equals("nullable", Comparison)).Value.ToString();
				property.IsNullable = isNullable.Equals("true", Comparison) ? true : false;
				verb.Parameters.Add(property);
			}
		}

		private static void AddRequestObjects(JsonProperty jsonVerb, Verb verb, List<Model> models)
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

		private static void AddResponseObjects(JsonProperty jsonVerb, Verb verb, List<Model> models, GenerationContext context)
		{
			var responses = jsonVerb.Value.EnumerateObject().FirstOrDefault(a => a.Name.Equals("responses", Comparison));
			if (responses.Value.ValueKind == JsonValueKind.Undefined)
			{
				return;
			}
			var twoHundredContentResponse = responses
										.Value.EnumerateObject()
										.FirstOrDefault(a => a.Name.StartsWith("20", Comparison))
										.Value.EnumerateObject()
										.FirstOrDefault(a => a.Name.Equals("content", Comparison));
			if (twoHundredContentResponse.Value.ValueKind == JsonValueKind.Undefined)
			{
				return;
			}
			// this condition is for referenced objects as return types
			var responseObject = twoHundredContentResponse
						.Value.EnumerateObject()
						.FirstOrDefault(a => a.Name.Equals("application/json", Comparison))
						.Value.EnumerateObject()
						.FirstOrDefault(a => a.Name.Equals("schema", Comparison))
						.Value.EnumerateObject()
						.FirstOrDefault(a => a.Name.Equals("$ref", Comparison));
			if (responseObject.Value.ValueKind != JsonValueKind.Undefined)
			{
				var responseObjectString = responseObject.Value.ToString();
				if (!string.IsNullOrEmpty(responseObjectString))
				{
					var responseObjectName = responseObjectString.Split('/').Last();
					verb.ResponseObject = models.FirstOrDefault(a => a.Name.Value.Equals(responseObjectName, Comparison));
				}
			}
			else
			{
				// this condition is for simple types
				var underlyingType = twoHundredContentResponse
						.Value.EnumerateObject()
						.FirstOrDefault(a => a.Name.Equals("application/json", Comparison))
						.Value.EnumerateObject()
						.FirstOrDefault(a => a.Name.Equals("schema", Comparison))
						.Value.EnumerateObject();
				var openApiType = GetOpenApiType(underlyingType, models);
				verb.ResponseType = TypeFactory.Create(openApiType, context.TypeConfiguration);
			}
		}

		private static List<Model> GetModels(JsonDocument document, GenerationContext context)
		{
			var models = new List<Model>();
			var componentsNode = document.RootElement.EnumerateObject()
								.FirstOrDefault(a => a.Name.Equals("components", Comparison));
			if (componentsNode.Value.ValueKind == JsonValueKind.Undefined)
			{
				return models;
			}
			var schemaNode = componentsNode
								.Value.EnumerateObject()
								.FirstOrDefault(a => a.Name.Equals("schemas", Comparison));
			if (schemaNode.Value.ValueKind == JsonValueKind.Undefined)
			{
				return models;
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
					var openApiType = GetOpenApiType(propertyProperties, models);
					property.Type = TypeFactory.Create(openApiType, context.TypeConfiguration);
					var isNullable = propertyProperties.FirstOrDefault(a => a.Name.Equals("nullable", Comparison)).Value.ToString();
					property.IsNullable = isNullable.Equals("true", Comparison) ? true : false;
					model.Properties.Add(property);
				}
				models.Add(model);
			}
			return models;
		}

		private static OpenApiType GetOpenApiType(JsonElement.ObjectEnumerator propertyProperties, List<Model> models)
		{
			var type = propertyProperties.FirstOrDefault(a => a.Name.Equals("type", Comparison));
			var typeString = "";
			if (type.Value.ValueKind == JsonValueKind.Undefined)
			{
				type = propertyProperties.FirstOrDefault(a => a.Name.Equals("$ref", Comparison));
				var requestObjectName = type.Value.ToString().Split('/').Last();
				typeString = models.FirstOrDefault(a => a.Name.Value.Equals(requestObjectName, Comparison)).Name.Value;
			}
			else
			{
				typeString = type.Value.ToString();
			}
			// these won't explode when null
			var typeFormat = propertyProperties.FirstOrDefault(a => a.Name.Equals("format", Comparison)).Value.ToString();
			var nullable = propertyProperties.FirstOrDefault(a => a.Name.Equals("nullable", Comparison)).Value.ToString();
			return new OpenApiType
			{
				Type = typeString,
				Format = typeFormat,
				Nullable = string.IsNullOrWhiteSpace(nullable)
					? false
					: BooleanConverter.ReferenceEquals(nullable, "false")
			};
		}

		private static List<Route> GetPaths(JsonDocument document, List<Model> models, GenerationContext context)
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
						AddParameters(verb, jsonParameters, context, models);
					}
					AddResponseObjects(jsonVerb, verb, models, context);
					route.Verbs.Add(verb);
				}
				result.Add(route);
			}
			return result;
		}
	}
}