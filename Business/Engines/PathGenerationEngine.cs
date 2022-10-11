using Business.Factories;
using Domain;
using Domain.Configuration;
using Domain.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Path = Domain.Models.Path;

namespace Business.Engines
{
    public class PathGenerationEngine 
	{
		private ILogger<PathGenerationEngine> _logger;

		private const StringComparison Comparison = StringComparison.InvariantCultureIgnoreCase;

		public PathGenerationEngine(
			ILogger<PathGenerationEngine> logger
		)
		{
			_logger = logger;
		}

		public OperationResult GeneratePaths(OpenAPIData openAPIData, GenerationContext context, Template template)
		{
			var options = new JsonDocumentOptions
			{
				CommentHandling = JsonCommentHandling.Skip
			};
			using (var document = JsonDocument.Parse(openAPIData.Data, options))
			{
				var result = new List<Path>();
				var pathsNode = document.RootElement.EnumerateObject()
									.First(a => a.Name.Equals("paths", Comparison));
				foreach (var pathNode in pathsNode.Value.EnumerateObject())
				{
					var path = new Path();
					path.Name = NameFactory.Create(pathNode.Name);
					path.Route = pathNode.Name;
					path.Namespace = template.Namespace;
					var verbs = pathNode.Value.EnumerateObject();
					foreach (var verb in verbs)
					{
						path.Verb = verb.Name;
						var jsonParameters = verb.Value.EnumerateObject().FirstOrDefault(a => a.Name.Equals("parameters", Comparison));
						if (jsonParameters.Value.ValueKind == JsonValueKind.Undefined)
						{
							var requestBody = verb.Value.EnumerateObject().FirstOrDefault(a => a.Name.Equals("requestBody", Comparison));
							if (requestBody.Value.ValueKind == JsonValueKind.Undefined)
							{
								continue;
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
						}
						else
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
								property.Type = string.IsNullOrWhiteSpace(typeFormat) ? type : typeFormat;
								var isNullable = propertyProperties.FirstOrDefault(a => a.Name.Equals("nullable", Comparison)).Value.ToString();
								property.IsNullable = isNullable.Equals("true", Comparison) ? true : false;
								path.Parameters.Add(property);
							}
							result.Add(path);
						}
					}
				}
			}
			return OperationResult.Ok();
		}
	}
}