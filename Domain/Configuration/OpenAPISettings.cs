using System.ComponentModel.DataAnnotations;

namespace Domain.Configuration
{
	public class OpenAPISettings
	{
		[Required]
		public string Location { get; set; }

		[Required]
		public string Name { get; set; }
	}
}