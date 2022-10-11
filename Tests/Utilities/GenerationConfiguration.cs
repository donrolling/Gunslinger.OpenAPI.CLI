using Domain.Configuration;
using Utilities.IO;

namespace Tests.Utilities
{
	public static class GenerationConfiguration
	{
		public static GenerationContext GetGenerationConfiguration(string pathToGenerationContext)
		{
			if (pathToGenerationContext.Contains(":"))
			{
				return FileUtility.ReadFileAsType<GenerationContext>(pathToGenerationContext).Result;
			}
			var currentDirectory = Directory.GetCurrentDirectory();
			return FileUtility.ReadFileAsType<GenerationContext>($"{currentDirectory}\\{pathToGenerationContext}").Result;
		}
	}
}