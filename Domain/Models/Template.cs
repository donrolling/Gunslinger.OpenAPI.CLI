using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
	public class Template
	{
		/// <summary>
		/// Needs to be the same as an existing DataProvider
		/// </summary>
		[Required]
		public string DataProviderName { get; set; }

		public bool DeleteAllItemsInOutputDirectory { get; set; } = true;

		public List<string> ExcludeTheseTypes { get; set; } = new List<string>();

		public List<string> Imports { get; set; } = new List<string>();
		
		/// <summary>
		/// This is the relative path of the template from the template directory root.
		/// </summary>
		[Required]
		public string InputRelativePath { get; set; }
		
		[Required]
		public ProgrammingLanguage Language { get; set; }

		/// <summary>
		/// Helps keep track of the templates.
		/// </summary>

		[Required]
		public string Name { get; set; }

		/// <summary>
		/// Every template will be output to a directory, ideally this namespace is similar or identical to that output directory.
		/// </summary>
		public string Namespace { get; set; } = "DefaultNamespace";

		/// <summary>
		/// This is the relative path to which the item will be output. You may use {entityName} or {pathName} to format the output.
		/// Example: "Entities\\{entityName}.cs",
		/// </summary>
		[Required]
		public string OutputRelativePath { get; set; }
		
		public string TemplateText { get; set; }

		/// <summary>
		/// Each template should choose a type: [Setup, Model, Path] Model is for types, Path is for endpoints, and Setup is for DI wire-up and the like (one per run, not one per item).
		/// </summary>
		[Required]
		public TemplateType Type { get; set; }
	}
}