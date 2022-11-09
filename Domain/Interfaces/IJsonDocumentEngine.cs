using Domain.Configuration;
using Domain.Models;

namespace Domain.Interfaces
{
	public interface IJsonDocumentEngine
	{
		Task<OperationResult<ApiDoc>> GetJsonDocAsync(GenerationContext context, string dataProviderName);
	}
}