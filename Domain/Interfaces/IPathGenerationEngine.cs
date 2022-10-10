using Domain.Configuration;
using Domain.Models;

namespace Domain.Interfaces
{
	public interface IPathGenerationEngine
	{
		Task<OperationResult> GeneratePathsAsync(OpenAPIData openAPIData, GenerationContext context, Template template);
	}
}