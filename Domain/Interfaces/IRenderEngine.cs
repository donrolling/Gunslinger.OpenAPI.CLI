using Domain.Models;
using Path = Domain.Models.Path;

namespace Domain.Interfaces
{
	public interface IRenderEngine
	{
		string Render(string template, Model model);

		string Render(string template, Path path);

		string Render(string template, ModelGroup modelGroup);
	}
}