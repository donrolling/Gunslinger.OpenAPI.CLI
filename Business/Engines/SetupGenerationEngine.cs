using Business.Factories;
using Domain;
using Domain.Configuration;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Business.Engines
{
	public class SetupGenerationEngine : ISetupGenerationEngine
	{
		private ILogger<SetupGenerationEngine> _logger;

		private const StringComparison Comparison = StringComparison.InvariantCultureIgnoreCase;

		public SetupGenerationEngine(
			ILogger<SetupGenerationEngine> logger
		)
		{
			_logger = logger;
		}

		public OperationResult GenerateSetup(OpenAPIData openAPIData, GenerationContext context, Template template)
		{
			var options = new JsonDocumentOptions
			{
				CommentHandling = JsonCommentHandling.Skip
			};
			using (var document = JsonDocument.Parse(openAPIData.Data, options))
			{
				var result = new List<Model>();
				var schemaNode = document
							.RootElement
							.EnumerateObject()
							.First(a => a.Name.Equals("components", Comparison))
							.Value
							.EnumerateObject()
							.First(a => a.Name.Equals("schemas", Comparison));
				foreach (var component in schemaNode.Value.EnumerateObject())
				{
					var model = new Model();
					model.Name = NameFactory.Create(component.Name);
					model.Namespace = template.Namespace;
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
						property.Type = string.IsNullOrWhiteSpace(typeFormat) ? type : typeFormat;
						var isNullable = propertyProperties.FirstOrDefault(a => a.Name.Equals("nullable", Comparison)).Value.ToString();
						property.IsNullable = isNullable.Equals("true", Comparison) ? true : false;
						model.Properties.Add(property);
					}
					result.Add(model);
				}
			}
			return OperationResult.Ok();
		}
	}
}