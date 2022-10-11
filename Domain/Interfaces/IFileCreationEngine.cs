using Domain.Configuration;
using Domain.Models;

namespace Domain.Interfaces
{
	public interface IFileCreationEngine
	{
		OperationResult CleanupOutputDirectory(string directory);
		
		Task<OperationResult> CreateFileAsync(string destinationPath, string output);		

		string PrepareOutputDirectory(GenerationContext context, Template template);
	}
}