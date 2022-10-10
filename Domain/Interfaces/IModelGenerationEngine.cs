using Domain.Configuration;
using Domain.Models;

namespace Domain.Interfaces
{
	public interface IModelGenerationEngine
	{
		Task<OperationResult> GenerateModelsAsync(OpenAPIData openAPIData, GenerationContext context, Template template);
	}
}