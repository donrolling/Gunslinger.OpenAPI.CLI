using Domain;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using Path = System.IO.Path;

namespace Business.Engines
{
	public class FileCreationEngine : IFileCreationEngine
	{
		private readonly List<string> _alreadyCleanedDirectories = new List<string>();
		private readonly ILogger<FileCreationEngine> _logger;

		public FileCreationEngine(ILogger<FileCreationEngine> logger)
		{
			_logger = logger;
		}

		public OperationResult CleanupOutputDirectory(string directory)
		{
			if (_alreadyCleanedDirectories.Contains(directory))
			{
				return OperationResult.Ok();
			}
			_alreadyCleanedDirectories.Add(directory);

			var di = new DirectoryInfo(directory);
			try
			{
				foreach (var file in di.GetFiles())
				{
					file.Delete();
				}
				foreach (var dir in di.GetDirectories())
				{
					dir.Delete(true);
				}
			}
			catch (Exception e)
			{
				return OperationResult.Fail($"Error FileOutputEngine.CleanupOutputDirectory: {e.Message}");
				throw;
			}
			return OperationResult.Ok();
		}

		public async Task<OperationResult> CreateFileAsync(string path, string output)
		{
			try
			{
				// prepare destination directory - todo: does this work if several directories need to be created?
				var destinationDirectory = path.Substring(0, path.LastIndexOf('\\'));
				Directory.CreateDirectory(destinationDirectory);
				await File.WriteAllTextAsync(path, output);
			}
			catch (Exception ex)
			{
				return OperationResult.Fail(ex.Message);
			}
			return OperationResult.Ok();
		}

		public OperationResult PrepareOutputDirectory(string path, bool deleteAllItemsInOutputDirectory)
		{
			if (!deleteAllItemsInOutputDirectory)
			{
				// don't clean the directory if DeleteAllItemsInOutputDirectory is set to false
				// this way a directory may contain elements that shouldn't be deleted.
				// This is not preferred, but it gives us flexibility.
				return OperationResult.Ok();
			}
			var destinationDirectory = Path.GetDirectoryName(path);
			if (deleteAllItemsInOutputDirectory)
			{
				return CleanupOutputDirectory(destinationDirectory);
			}
			return OperationResult.Ok();
		}

		private static string makeValidFileName(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return string.Empty;
			}
			var invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
			var invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);
			return Regex.Replace(name, invalidRegStr, "_");
		}
	}
}