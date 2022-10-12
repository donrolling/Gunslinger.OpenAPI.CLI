using Domain.Interfaces;

namespace Domain.Models
{
	public class Route : IRenderable
	{
		public List<string> Imports { get; set; }

		public Name Name { get; set; }

		public string Namespace { get; set; }

		public string Path { get; set; }

		public List<Verb> Verbs { get; set; } = new List<Verb>();
	}
}