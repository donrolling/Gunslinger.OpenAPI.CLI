using Domain;
using Microsoft.Extensions.FileProviders;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Utilities.IO
{
	public class FileUtility
	{
		private const string physicalBinDir = "\\bin\\";
		private const char physicalPathDelimiter = '\\';
		private const string webBinDir = "/bin/";
		private const char webPathDelimiter = '/';
		private static string _rootBinPath = "";

		public static string RootBinPath
		{
			get
			{
				if (string.IsNullOrEmpty(_rootBinPath))
				{
					_rootBinPath = Path.GetDirectoryName(typeof(FileUtility).Assembly.GetName().CodeBase);
				}
				return _rootBinPath;
			}
		}

		public static string CleanFilename(string filename, string replaceWith = "")
		{
			var regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
			var r = new Regex($"[{Regex.Escape(regexSearch)}]");
			return r.Replace(filename, replaceWith);
		}

		public static string GetBasePath<T>(bool fileResidesInBin) where T : class
		{
			var basePath = Path.GetDirectoryName(typeof(T).Assembly.Location);
			if (fileResidesInBin)
			{
				return basePath;
			}
			var pos = basePath.IndexOf("\\bin", StringComparison.Ordinal);
			if (pos >= 0)
			{
				basePath = basePath.Substring(0, pos);
			}
			return basePath;
		}

		public static string GetBasePath<T>() where T : class
		{
			var basePath = Path.GetDirectoryName(typeof(T).Assembly.Location);
			var pos = basePath.IndexOf("\\bin");
			if (pos >= 0)
			{
				basePath = basePath.Substring(0, pos);
			}
			return basePath;
		}

		public static string GetFullPath<T>(string subDirectory) where T : class
		{
			var path = string.Concat(GetBasePath<T>(), subDirectory);
			return path;
		}

		public static (string path, string filePath) GetFullPath_FromRelativePath<T>(string filename, string subDirectory = "") where T : class
		{
			var basePath = GetBasePath<T>(false);
			var path = string.IsNullOrEmpty(subDirectory) ? basePath : ProperPathCombine.Combine(new List<string> { basePath, subDirectory });
			var filePath = ProperPathCombine.Combine(new List<string> { path, filename });
			return (path, filePath);
		}

		public static FileStream OpenRead<T>(string filename, string subDirectory, bool fileResidesInBin = false) where T : class
		{
			var basePath = GetBasePath<T>(fileResidesInBin);
			var path = ProperPathCombine.Combine(new List<string> { basePath, subDirectory, filename });
			return File.OpenRead(path);
		}

		public static FileStream OpenRead<T>(string filename, string subDirectory) where T : class
		{
			if (subDirectory.Contains(":"))
			{
				var path = ProperPathCombine.Combine(new List<string> { subDirectory, filename });
				return File.OpenRead(path);
			}
			else
			{
				var basePath = GetBasePath<T>();
				var path = ProperPathCombine.Combine(new List<string> { basePath, subDirectory, filename });
				return File.OpenRead(path);
			}
		}

		public static OperationResult<T> ReadFileAsType<T>(string path) where T : class
		{
			var exists = File.Exists(path);
			if (!exists)
			{
				return OperationResult.Fail<T>("Configuration not found.");
			}
			var fileContents = File.ReadAllText(path);
			var options = new JsonSerializerOptions
			{
				WriteIndented = true,
				Converters = {
					new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
				}
			};
			var result = JsonSerializer.Deserialize<T>(fileContents, options);
			return OperationResult.Ok(result);
		}

		public static IEnumerable<string> ReadLines<T>(string filename, string subDirectory) where T : class
		{
			var basePath = GetBasePath<T>(false);
			var path = ProperPathCombine.Combine(new List<string> { basePath, subDirectory, filename });
			var result = new List<string>();
			using (var file = new StreamReader(path))
			{
				var counter = 0;
				var line = string.Empty;
				while ((line = file.ReadLine()) != null)
				{
					if (!string.IsNullOrEmpty(line))
					{
						result.Add(line);
					}
					counter++;
				}
			}
			return result;
		}

		/// <summary>
		/// Reads a text file in total.
		/// </summary>
		/// <typeparam name="T">A type contained in the source assembly from which you'd like to read the file.</typeparam>
		/// <param name="filename">Filename, duh</param>
		/// <param name="subDirectory">SubDirectory within the source assembly.</param>
		/// <returns></returns>
		public static string ReadTextFile<T>(string filename, string subDirectory, bool fileResidesInBin = false) where T : class
		{
			var basePath = GetBasePath<T>(fileResidesInBin);
			var path = ProperPathCombine.Combine(new List<string> { basePath, subDirectory, filename }); ;
			var data = File.ReadAllText(path);
			return data;
		}

		public static string ReadTextFile(string filename, string baseServerPath, string subDirectory)
		{
			var path = ProperPathCombine.Combine(new List<string> { baseServerPath, subDirectory, filename });
			var data = File.ReadAllText(path);
			return data;
		}

		/// <summary>
		/// Reads a text file in total.
		/// </summary>
		/// <typeparam name="T">A type contained in the source assembly from which you'd like to read the file.</typeparam>
		/// <param name="filename">Filename, duh</param>
		/// <param name="subDirectory">SubDirectory within the source assembly.</param>
		/// <returns></returns>
		public static async Task<string> ReadTextFileAsync<T>(string filename, string subDirectory) where T : class
		{
			var path = Path.Combine(GetBasePath<T>(), subDirectory, filename);
			return await ReadAllTextAsync(path);
		}

		public static async Task<string> ReadAllTextAsync(string filePath)
		{
			if (filePath.Contains(":\\"))
			{
				return await File.ReadAllTextAsync(filePath);
			}
			var path = string.Concat(GetBasePath<FileUtility>(), filePath);
			return await File.ReadAllTextAsync(path);
		}

		/// <summary>
		/// Don't specify bin, we'll take care of that for you.
		/// </summary>
		/// <param name="fileProvider"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string ReadTextFileFromBin(IFileProvider fileProvider, string path)
		{
			//protect us from ourselves
			var tidyPath = path
							.TrimStart(webPathDelimiter)
							.TrimStart(physicalPathDelimiter)
							.Replace(webBinDir, string.Empty)
							.Replace(physicalBinDir, string.Empty);
			//if this is on web we want it to work the same in a unit test
			var basePathSplit = RootBinPath.Split(physicalBinDir.ToCharArray());
			var partialPath = basePathSplit.Length > 1
				? $"{physicalBinDir}{basePathSplit[1]}"
				: string.Empty;
			var safePath = $"{partialPath}\\{tidyPath}".Replace(physicalPathDelimiter, webPathDelimiter);
			//protecting us from web vs file system reading issues
			if (fileProvider.GetFileInfo(safePath).Exists)
			{
				return readFileViaProvider(fileProvider, safePath);
			}
			else
			{
				safePath = $"{tidyPath}".Replace(physicalPathDelimiter, webPathDelimiter);
				return readFileViaProvider(fileProvider, safePath);
			}
		}

		public static void WriteFile<T>(string filename, string subDirectory, byte[] bytes) where T : class
		{
			var path = GetFullPath_FromRelativePath<T>(filename, subDirectory);
			Directory.CreateDirectory(path.path);
			File.WriteAllBytes(path.filePath, bytes);
		}

		public static void WriteFile<T>(string filename, string subDirectory, string contents) where T : class
		{
			var path = GetFullPath_FromRelativePath<T>(CleanFilename(filename, "_"), subDirectory);
			Directory.CreateDirectory(path.path);
			File.WriteAllText(path.filePath, contents);
		}

		public static void WriteFile<T>(string filename, string subDirectory, Stream contents) where T : class
		{
			var path = GetFullPath_FromRelativePath<T>(CleanFilename(filename, "_"), subDirectory);
			Directory.CreateDirectory(path.path);
			var output = new FileStream(path.filePath, FileMode.Create);
			contents.CopyTo(output);
		}

		/// <summary>
		/// Writes a stream to a file. Checks that the directory exists first.
		/// </summary>
		/// <param name="path">Path without filename</param>
		/// <param name="filename">Filename with extension</param>
		/// <param name="stream">Content stream</param>
		public static void WriteFile(string path, string filename, Stream stream)
		{
			// for safety, just trim the end up
			path = path.TrimEnd(new char[] { '\\' });
			var pathExists = Directory.Exists(path);
			if (!pathExists)
			{
				Directory.CreateDirectory(path);
			}
			var fullPath = $"{path}\\{filename}";
			using (var file = File.Create(fullPath))
			{
				stream.Seek(0, SeekOrigin.Begin);
				stream.CopyTo(file);
			}
		}

		public static void WriteFile(string path, string contents)
		{
			if (!Directory.Exists(path))
			{
				var dirName = Path.GetDirectoryName(path);
				Directory.CreateDirectory(dirName);
			}
			File.WriteAllText(path, contents);
		}

		private static string readFileViaProvider(IFileProvider fileProvider, string safePath)
		{
			using (var stream = fileProvider.GetFileInfo(safePath).CreateReadStream())
			using (var reader = new StreamReader(stream, Encoding.UTF8))
			{
				var sb = new StringBuilder();
				string line;
				while ((line = reader.ReadLine()) != null)
				{
					sb.AppendLine(line);
				}
				return sb.ToString();
			}
		}
	}
}