using Domain;
using Domain.Configuration;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Utilities.IO;
using IFileTemplateProvider = Domain.Interfaces.IFileTemplateProvider;

namespace Business.Factories
{
	public class ContextFactory : IContextFactory
	{
		private readonly AppSettings _appSettings;
		private readonly IFileTemplateProvider _fileProvider;
		private readonly ILogger<ContextFactory> _logger;

		public ContextFactory(
			IOptions<AppSettings> appSettings,
			IFileTemplateProvider fileProvider,
			ILogger<ContextFactory> logger
		)
		{
			_appSettings = appSettings.Value;
			_fileProvider = fileProvider;
			_logger = logger;
		}

		public OperationResult<GenerationContext> Create(CommandSettings commandSettings)
		{
			var configFilePath = GetConfigurationPath(commandSettings);
			var configPath = Path.GetDirectoryName(configFilePath);
			var generationContextReadResult = FileUtility.ReadFileAsType<GenerationContext>(configFilePath);
			if (generationContextReadResult.Failed)
			{
				// log failure and stop
				var msg = $"Could not find configuration file. Searched here: {configFilePath}.{Environment.NewLine}Type 'gs --help' for help.";
				return OperationResult.Fail<GenerationContext>(msg);
			}
			var generationContext = generationContextReadResult.Result;
			generationContext.RootPath = commandSettings.RootPath;
			generationContext.TemplateDirectory = FixPath(configPath, generationContext.TemplateDirectory);

			ReadTemplateText(generationContext);
			return OperationResult.Ok(generationContext);
		}

		private void ReadTemplateText(GenerationContext generationContext)
		{
			// read the template text for all of the templates
			foreach (var template in generationContext.Templates)
			{
				var templatePath = $"{generationContext.TemplateDirectory}\\{template.InputRelativePath}";
				var text = _fileProvider.Get(templatePath);
				if (string.IsNullOrEmpty(text))
				{
					throw new Exception($"Template was empty: {template.InputRelativePath}");
				}
				template.TemplateText = text;
			}
		}

		private static string FixPath(string rootPath, string path)
		{
			// if the user set either of these paths to a full path, then nothing needs to happen
			// but we need the full path, so prepend the root directory to any relative paths
			return path.Contains(":\\") ? path : $"{rootPath}\\{path}";
		}

		private string GetConfigurationPath(CommandSettings commandSettings)
		{
			return string.IsNullOrWhiteSpace(commandSettings.ConfigPath)
				? Path.Combine(commandSettings.RootPath, _appSettings.DefaultConfigFileName)
				: commandSettings.ConfigPath.Contains(Path.DirectorySeparatorChar)
					? File.Exists(commandSettings.ConfigPath)
						? commandSettings.ConfigPath
						: Path.Combine(commandSettings.ConfigPath, _appSettings.DefaultConfigFileName)
					: Path.Combine(commandSettings.RootPath, commandSettings.ConfigPath);
		}
	}
}