namespace Domain.Models
{
	public class Verb
	{
		public string Name { get; set; }

		public List<Property> Parameters { get; set; } = new List<Property>();

		public Model RequestObject { get; set; }

		public Model ResponseObject { get; set; }
	}
}