using Domain.Configuration;

namespace Domain.Interfaces
{
	public interface IGenerationManager
	{
		GenerationContext Context { get; }

		Task<OperationResult> GenerateAsync(CommandSettings commandSettings);
	}
}