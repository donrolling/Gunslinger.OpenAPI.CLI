using Domain.Configuration;
using Domain.Models;

namespace Domain.Interfaces
{
	public interface ISetupGenerationEngine
	{
		Task<OperationResult> GenerateSetupAsync(OpenAPIData openAPIData, GenerationContext context, Template template);
	}
}