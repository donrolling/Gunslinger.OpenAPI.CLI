namespace Domain.Interfaces
{
	public interface IRenderable : INamed
	{
		public List<string> Imports { get; set; }

		public string Namespace { get; set; }
	}
}