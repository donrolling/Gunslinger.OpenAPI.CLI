using Domain.Configuration;
using Domain.Models;

namespace Domain.Interfaces
{
	public interface IGenerationEngine
	{
		Task<OperationResult> GenerateModelsAsync(GenerationContext context, Template template);

		Task<OperationResult> GeneratePathsAsync(GenerationContext context, Template template);

		Task<OperationResult> GenerateSetupAsync(GenerationContext context, Template template);
	}
}