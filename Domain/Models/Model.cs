namespace Domain.Models
{
	public class Model
	{
		public List<Property> Properties { get; set; } = new List<Property>();
		public Name Name { get; set; }
		public string Namespace { get; set; }
		public string Description { get; set; }
	}
}