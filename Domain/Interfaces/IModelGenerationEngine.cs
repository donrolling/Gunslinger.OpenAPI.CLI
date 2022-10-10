using Domain.Configuration;
using Domain.Models;

namespace Domain.Interfaces
{
	public interface IModelGenerationEngine
	{
		Task<OperationResult> GenerateModels(GenerationContext context, Template template);
	}
}