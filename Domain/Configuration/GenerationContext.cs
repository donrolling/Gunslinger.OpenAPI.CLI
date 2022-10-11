using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Domain.Configuration
{
	public class GenerationContext
	{
		public string ConfigPath { get; set; }

		[Required]
		public List<OpenAPISettings> DataProviders { get; set; } = new List<OpenAPISettings>();

		public List<string> ExcludeTheseEntities { get; set; } = new List<string>();

		public List<string> ExcludeTheseTemplates { get; set; } = new List<string>();

		public List<string> IncludeTheseEntitiesOnly { get; set; } = new List<string>();

		public List<string> IncludeTheseTemplatesOnly { get; set; } = new List<string>();

		public string RootPath { get; set; }

		[Required]
		public string TemplateDirectory { get; set; }

		public List<Template> Templates { get; set; } = new List<Template>();
	}
}