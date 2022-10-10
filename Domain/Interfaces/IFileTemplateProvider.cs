using Domain.Configuration;

namespace Domain.Interfaces
{
	public interface IFileTemplateProvider
	{
		string Get(GenerationContext context, string filename);

		string Get(string filename);
	}
}