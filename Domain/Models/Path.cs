using Domain.Interfaces;

namespace Domain.Models
{
	public class Path : IRenderable
	{
		public List<string> Imports { get; set; }

		public Name Name { get; set; }

		public string Namespace { get; set; }

		public string Route { get; set; }

		public List<Verb> Verbs { get; set; } = new List<Verb>();
	}
}