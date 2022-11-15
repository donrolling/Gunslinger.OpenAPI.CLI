namespace Domain.Models
{
	public class Name
	{
		public string Value { get; set; }
		public string NameWithSpaces { get; set; }
		public string LowerCamelCase { get; set; }
		public string PascalCase { get; set; }
		public string UpperCase { get; set; }

		/// <summary>
		/// This is a version of the name that is safe for method names
		/// </summary>
		public Name Safe { get; set; }
		
		public Name CollisionSafe { get; set; }
	}
}