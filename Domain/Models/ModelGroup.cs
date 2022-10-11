using Domain.Interfaces;

namespace Domain.Models
{
	public class ModelGroup : INamed
	{
		public OpenApiResult Data { get; set; }
		
		public List<string> Imports { get; set; }
		
		public Name Name { get; set; }

		public string Namespace { get; set; }
	}
}