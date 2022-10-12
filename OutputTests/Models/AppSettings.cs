using System.ComponentModel.DataAnnotations;

namespace OutputTests.Models
{
	public class AppSettings
	{
		[Required]
		public string LogFilePath { get; set; }
	}
}