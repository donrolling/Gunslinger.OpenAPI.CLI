namespace Domain.Models
{
	public class Verb
	{
		public Name Name { get; set; }

		public bool ParametersAreEmpty { get => Parameters == null || !Parameters.Any(); }

		public List<Property> Parameters { get; set; } = new List<Property>();

		public bool RequestObjectIsEmpty { get => RequestObject == null; }

		public Model RequestObject { get; set; }

		public bool ResponseObjectIsEmpty { get => ResponseObject == null; }

		public Model ResponseObject { get; set; }
	}
}