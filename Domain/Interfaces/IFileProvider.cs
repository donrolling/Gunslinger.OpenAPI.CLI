using Domain.Configuration;

namespace Domain.Interfaces
{
	public interface IFileProvider
	{
		string Get(GenerationContext context, string filename);

		string Get(string filename);
	}
}