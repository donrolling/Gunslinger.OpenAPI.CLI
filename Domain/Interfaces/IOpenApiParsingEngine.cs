using Domain.Configuration;
using Domain.Models;

namespace Domain.Interfaces
{
	public interface IOpenApiParsingEngine
	{
		Task<OperationResult<OpenApiResult>> ParseOpenApiAsync(GenerationContext context, string dataProviderName);
	}
}