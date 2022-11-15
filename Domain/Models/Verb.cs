namespace Domain.Models
{
	public class Verb
	{
		public Name Name { get; set; }

		public List<Property> Parameters { get; set; } = new List<Property>();

		public bool ParametersAreEmpty { get => Parameters == null || !Parameters.Any(); }
		
		public bool ParametersAreNotEmpty { get => Parameters != null && Parameters.Any(); }

		public List<Property> PathParameters { get; set; } = new List<Property>();

		public bool PathParametersAreEmpty { get => PathParameters == null || !PathParameters.Any(); }

		public bool PathParametersAreNotEmpty { get => PathParameters != null && PathParameters.Any(); }

		public Model RequestObject { get; set; }
		
		public bool RequestObjectIsEmpty { get => RequestObject == null; }
		
		public Model ResponseObject { get; set; }
		
		public bool ResponseObjectIsEmpty { get => ResponseObject == null; }
	}
}