using Domain;
using Domain.Configuration;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Utilities.IO;
using IFileProvider = Domain.Interfaces.IFileProvider;

namespace Gunslinger.OpenAPI.CLI.Factories
{
	public class ContextFactory : IContextFactory
	{
		private readonly AppSettings _appSettings;
		private readonly IFileProvider _fileProvider;
		private readonly ILogger<ContextFactory> _logger;

		public ContextFactory(
			IOptions<AppSettings> appSettings,
			IFileProvider fileProvider,
			ILogger<ContextFactory> logger
		)
		{
			_appSettings = appSettings.Value;
			_fileProvider = fileProvider;
			_logger = logger;
		}

		public OperationResult<GenerationContext> Create(CommandSettings commandSettings)
		{
			var configPath = GetConfigurationPath(commandSettings);
			var generationContextReadResult = FileUtility.ReadFileAsType<GenerationContext>(configPath);
			if (generationContextReadResult.Failed)
			{
				// log failure and stop
				var msg = $"Could not find configuration file. Searched here: {configPath}.{Environment.NewLine}Type 'gs --help' for help.";
				return OperationResult.Fail<GenerationContext>(msg);
			}
			var generationContext = generationContextReadResult.Result;
			generationContext.OutputDirectory = FixPath(commandSettings.RootPath, generationContext.OutputDirectory);
			generationContext.RootPath = commandSettings.RootPath;
			generationContext.TemplateDirectory = FixPath(commandSettings.RootPath, generationContext.TemplateDirectory);

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
					throw new Exception($"Template was empty: {template.Name}");
				}
				template.TemplateText = text;
				// now add the template to the generation context
				generationContext.Templates.Add(template);
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