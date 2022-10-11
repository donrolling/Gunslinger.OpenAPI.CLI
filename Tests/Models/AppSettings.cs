using System.ComponentModel.DataAnnotations;

namespace Tests.Models
{
	public class AppSettings
	{
		[Required]
		public string LogFilePath { get; set; }
	}
}