using System.ComponentModel.DataAnnotations;

namespace IntegrationTestProject.Models
{
	public class AppSettings
	{
		[Required]
		public string LogFilePath { get; set; }

		[Required]
		public string BaseUrl { get; set; }
	}
}