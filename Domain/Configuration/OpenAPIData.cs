using Domain.Models;
using Path = Domain.Models.Path;

namespace Domain.Configuration
{
	public class OpenAPIData : OpenAPISettings
	{
		public string Data { get; set; }
		public List<Model> Models { get; set; }
		public List<Path> Paths { get; set; }
	}
}