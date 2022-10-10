using Domain.Configuration;
using Domain.Models;

namespace Domain.Interfaces
{
	public interface IGenerationEngine
	{
		Task<OperationResult> GenerateModels(GenerationContext context, Template template);

		Task<OperationResult> GeneratePaths(GenerationContext context, Template template);

		Task<OperationResult> GenerateSetup(GenerationContext context, Template template);
	}
}