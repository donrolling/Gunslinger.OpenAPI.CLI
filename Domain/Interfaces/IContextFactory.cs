using Domain.Configuration;

namespace Domain.Interfaces
{
	public interface IContextFactory
	{
		OperationResult<GenerationContext> Create(CommandSettings commandSettings);
	}
}