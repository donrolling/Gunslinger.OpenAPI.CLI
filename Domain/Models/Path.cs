namespace Domain.Models
{
	public class Path
	{
		public string Namespace { get; set; }

		public List<Property> Parameters { get; set; } = new List<Property>();

		public string Route { get; set; }

		public string Verb { get; set; }
	}
}