namespace Domain.Models
{
	public class Property
	{
		public string Description { get; set; }
		public string Type { get; set; }
		public Name Name { get; set; }
		public Name ModelName { get; set; }
		public bool IsNullable { get; set; }
	}
}