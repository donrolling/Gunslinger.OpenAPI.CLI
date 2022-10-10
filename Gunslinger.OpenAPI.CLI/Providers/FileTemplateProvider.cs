using Domain.Configuration;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Gunslinger.OpenAPI.CLI.Providers
{
	public class FileTemplateProvider : IFileProvider
	{
		private readonly ILogger<FileTemplateProvider> _logger;

		public FileTemplateProvider(ILogger<FileTemplateProvider> logger)
		{
			_logger = logger;
		}

		public string Get(GenerationContext context, string filename)
		{
			return Get($"{context.TemplateDirectory}\\{filename}");
		}

		public string Get(string fullPath)
		{
			try
			{
				return File.ReadAllText(fullPath);
			}
			catch (Exception e)
			{
				_logger.LogError($"FileTemplateProvider Get({fullPath})\r\nError: {e.Message}");
				return string.Empty;
			}
		}
	}
}