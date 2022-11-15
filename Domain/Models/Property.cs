namespace Domain.Models
{
	public class Property
	{
		/// <summary>
		/// Can be used in a template to avoid naming a property the same name as a method
		/// </summary>
		public bool HasSameNameAsType { get; set; }

		public bool IsNullable { get; set; }

		public Name Name { get; set; }

		public string Type { get; set; }
	}
}