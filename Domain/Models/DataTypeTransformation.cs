namespace Domain.Models
{
	public class DataTypeTransformation
	{
		public string DataType { get; set; }
		public string DestinationType { get; set; }
		public string Format { get; set; }
		public bool Nullable { get; set; }
		public DataTypeTransformation Items { get; set; }
	}
}