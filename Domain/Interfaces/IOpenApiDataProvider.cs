using Domain.Configuration;
using Domain.Models;
using Path = Domain.Models.Path;

namespace Domain.Interfaces
{
	public interface IOpenApiDataProvider
	{
		Task<OperationResult<Dictionary<string, Model>>> GetModels(GenerationContext generationContext, Template template, OpenAPISettings settings);

		Task<OperationResult<Dictionary<string, Path>>> GetPaths(GenerationContext generationContext, Template template, OpenAPISettings settings);
	}
}