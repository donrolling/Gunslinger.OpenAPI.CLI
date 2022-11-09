using Domain.Enums;
using System.Text.Json;

namespace Domain.Models
{
	public class ApiDoc
	{
		public JsonDocument JsonDocument { get; set; }

		public SupportedDocTypes SupportedDocTypes { get; set; }
	}
}