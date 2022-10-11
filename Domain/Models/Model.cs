using Domain.Interfaces;

namespace Domain.Models
{
	public class Model : IRenderable
	{
		public List<string> Imports { get; set; }

		public Name Name { get; set; }

		public string Namespace { get; set; }

		public List<Property> Properties { get; set; } = new List<Property>();

		public string TypeName { get; set; }
	}
}