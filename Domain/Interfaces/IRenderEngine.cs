namespace Domain.Interfaces
{
	public interface IRenderEngine
	{
		OperationResult<string> Render(string template, IRenderable item);
	}
}