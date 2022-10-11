using Domain.Configuration;
using Domain.Models;

namespace Domain.Interfaces
{
	public interface ISetupGenerationEngine
	{
		OperationResult GenerateSetup(OpenAPIData openAPIData, GenerationContext context, Template template);
	}
}