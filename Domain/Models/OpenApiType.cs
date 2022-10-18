using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
	public class OpenApiType
	{
		[Required]
		public string Type { get; set; }

		public string Format { get; set; }
		public bool Nullable { get; set; }
		public OpenApiArrayType Items { get; set; }
	}
}