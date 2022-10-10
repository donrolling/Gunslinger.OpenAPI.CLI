//using Domain;
//using Domain.Configuration;
//using Domain.Interfaces;
//using Domain.Models;
//using Gunslinger.OpenAPI.CLI.Factories;
//using Microsoft.Extensions.Logging;
//using Newtonsoft.Json.Linq;
//using Serilog.Core;
//using System.Net.Http;
//using Utilities.IO;
//using Path = Domain.Models.Path;

//namespace Gunslinger.OpenAPI.CLI.Providers
//{
//	public class OpenApiDataProvider : IOpenApiDataProvider
//	{
//		private readonly IHttpClientFactory _httpClientFactory;
//		private readonly ILogger<OpenApiDataProvider> _logger;

//		public OpenApiDataProvider(
//			IHttpClientFactory httpClientFactory,
//			ILogger<OpenApiDataProvider> logger
//		)
//		{
//			_httpClientFactory = httpClientFactory;
//			_logger = logger;
//		}

//		public async Task<OperationResult<Dictionary<string, Model>>> GetModels(GenerationContext generationContext, Template template, OpenAPISettings settings)
//		{
//			var dataResult = getData(generationContext);
//			if (dataResult.Failed)
//			{
//				return OperationResult.Fail<Dictionary<string, Model>>(dataResult.Message);
//			}
//			var data = dataResult.Result;
//			try
//			{
//				var root = JToken.Parse(data);
//				var definitions = root["definitions"].Value<JObject>();
//				var items = new Dictionary<string, Model>();

//				var whiteList = generationContext.IncludeTheseEntitiesOnly.Any();
//				var blackList = generationContext.ExcludeTheseEntities.Any();

//				foreach (var (key, value) in definitions)
//				{
//					// shouldn't be both, but we're going to try to be nice
//					if (whiteList)
//					{
//						if (!generationContext.IncludeTheseEntitiesOnly.Contains(key))
//						{
//							continue;
//						}
//					}
//					if (blackList)
//					{
//						if (generationContext.ExcludeTheseEntities.Contains(key))
//						{
//							continue;
//						}
//					}

//					var item = parseItem(template.Namespace, key, value, template);
//					if (item == null) continue;
//					items.Add(key, item);
//				}

//				return OperationResult.Ok(items);
//			}
//			catch (Exception ex)
//			{
//				return OperationResult.Fail<Dictionary<string, Model>>($"Swagger Data Provider - Failed to parse data from provider.\r\n\t{ex.Message}\r\n\tData: {data}");
//			}
//		}

//		public async Task<OperationResult<Dictionary<string, Path>>> GetPaths(GenerationContext generationContext, Template template, OpenAPISettings settings)
//		{
//			var dataResult = await GetData(generationContext, settings);
//			if (dataResult.Failed)
//			{
//				return OperationResult.Fail<Dictionary<string, Path>>(dataResult.Message);
//			}
//			var data = dataResult.Result;
//			try
//			{
//				var root = JToken.Parse(data);

//				return OperationResult<Dictionary<string, Path>>.Ok(items);
//			}
//			catch (Exception ex)
//			{
//				return OperationResult.Fail<Dictionary<string, Path>>($"Swagger Data Provider - Failed to parse data from provider.\r\n\t{ex.Message}\r\n\tData: {data}");
//			}
//		}

//		private string fixCollectionType(JToken value)
//		{
//			if (value["items"] == null) throw new Exception("Unknown parsing situation. Items node was empty.");
//			if (value["items"]["type"] == null && value["items"]["$ref"] == null) throw new Exception("Unknown parsing situation. Items node had no type or $ref property.");

//			if (value["items"]["type"] != null)
//			{
//				var type = value["items"]["type"];
//				return $"List<{type}>";
//			}
//			else if (value["items"]["$ref"] != null)
//			{
//				var referenceDefinition = value["items"]["$ref"].ToString().Replace(RefDef, string.Empty);
//				return $"List<{referenceDefinition}>";
//			}

//			throw new Exception("Unknown parsing situation. Items node had no type or $ref property.");
//		}

//		private async Task<OperationResult<string>> GetData(GenerationContext generationContext, OpenAPISettings openAPISettings)
//		{
//			if (string.IsNullOrEmpty(openAPISettings.Location))
//			{
//				return OperationResult.Fail<string>("Swagger DataSource is not specified.");
//			}
//			// datasource could be an url or a path
//			var isWellFormedUriString = Uri.IsWellFormedUriString(openAPISettings.DataSource, UriKind.Absolute);
//			if (isWellFormedUriString)
//			{
//				try
//				{
//					var response = await _httpClientFactory.CreateClient().GetAsync(location);
//					var content = await response.Content.ReadAsStringAsync();
//					return OperationResult.Ok(content);
//				}
//				catch (Exception ex)
//				{
//					var message = $"DataProvider failed to make an http call to: {_dataProviderSettings.DataSource}.\r\n{ex.Message}";
//					_logger.LogError(message);
//					return OperationResult.Fail<string>(message);
//				}
//			}
//			else
//			{
//				try
//				{
//					// give us a full path if there isn't one already
//					if (!_dataProviderSettings.DataSource.Contains(":\\"))
//					{
//						_dataProviderSettings.DataSource = $"{generationContext.RootPath}\\{_dataProviderSettings.DataSource}";
//					}
//					var result = FileUtility.ReadTextFile(_dataProviderSettings.DataSource);
//					return OperationResult.Ok(result);
//				}
//				catch (Exception ex)
//				{
//					var message = $"DataProvider failed to read file: {_dataProviderSettings.DataSource}.\r\n{ex.Message}";
//					Logger.LogError(message);
//					return OperationResult.Fail<string>(message);
//				}
//			}
//		}

//		private string getDataType(JToken value, bool isNullable)
//		{
//			//safety. some of these didn't have a type defined.
//			if (value["type"] == null)
//			{
//				// is there something else that we can do here?
//				return "string";
//			}
//			var format = string.Empty;
//			if (value["format"] != null)
//			{
//				format = value["format"].ToString(); //"format": "date-time";
//			}

//			var type = value["type"].ToString();
//			switch (type)
//			{
//				case "integer":
//					return isNullable == true ? "int?" : "int";

//				case "array":
//					return fixCollectionType(value);

//				case "string":
//					switch (format)
//					{
//						case "date-time":
//							return isNullable ? "DateTime?" : "DateTime";

//						default:
//							return type;
//					}

//				case "number":
//					if (!string.IsNullOrEmpty(format))
//					{
//						return isNullable == true ? $"{format}?" : format;
//					}
//					// what should happen here?
//					// setting to a double for now.
//					// sometimes Swagger doesn't give us much to go on here. Can we talk to Dustin about ways to fix this?
//					return isNullable == true ? "double?" : "double";

//				default:
//					return type;
//			}
//		}

//		private bool getNullableStatus(List<string> requiredProperties, string key, JToken value)
//		{
//			// required properties could be used to determine type, but it doesn't seem like that is the intended use
//			// if you wanted to do that, use this code:
//			//      var isRequired = requiredProperties.Contains(key);
//			// Instead, under normal circumstances we should use the nullable property
//			if (value["nullable"] == null)
//			{
//				// I made a setting that allows the engine to interpret the lack of
//				// a nullable specification as a request for a nullable property.
//				// That is odd, but here we are.
//				if (_dataProviderSettings.NonSpecifiedPropertiesAreNullable)
//				{
//					// special default without a nullable specification would be that the
//					// property is nullable
//					return true;
//				}
//				else
//				{
//					// standard default without a nullable specification would be that the
//					// property is NOT nullable
//					return false;
//				}
//			}
//			var nullable = value["nullable"].ToString() == "true";
//			return nullable;
//		}

//		private Property getProperty(List<string> requiredProperties, string key, JToken value, Template template)
//		{
//			var isNullable = getNullableStatus(requiredProperties, key, value);
//			var type = getDataType(value, isNullable);
//			var propertyDescription = value["description"] != null ? value["description"].ToString() : string.Empty;
//			var prop = new Property
//			{
//				Name = NameFactory.Create(key, template, false),
//				Type = type,
//				Description = propertyDescription,
//				IsNullable = isNullable
//			};
//			return prop;
//		}

//		private Model parseItem(string _namespace, string className, JToken definition, Template template)
//		{
//			var item = new Model
//			{
//				Namespace = _namespace,
//				Name = NameFactory.Create(className)
//			};
//			try
//			{
//				item.Description = definition["description"] != null ? definition["description"].ToString() : string.Empty;
//				var requiredProperties = new List<string>();
//				if (definition["required"] != null)
//				{
//					requiredProperties = definition["required"].Value<JArray>().Select(a => a.ToString()).ToList();
//				}
//				if (definition["properties"] == null)
//				{
//					//some definitions don't have properties, we're just going to skip those
//					return null;
//				}
//				var properties = definition["properties"].Value<JObject>();
//				foreach (var (key, value) in properties)
//				{
//					var prop = getProperty(requiredProperties, key, value, template);
//					item.Properties.Add(prop);
//				}

//				return item;
//			}
//			catch (Exception ex)
//			{
//				var msg = ex.ToString();
//				return null;
//			}
//		}
//	}
//}