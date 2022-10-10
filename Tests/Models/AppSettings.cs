using System.ComponentModel.DataAnnotations;

namespace IntegrationTestProject.Models
{
	public class AppSettings
	{
		[Required]
		public string LogFilePath { get; set; }
	}
}