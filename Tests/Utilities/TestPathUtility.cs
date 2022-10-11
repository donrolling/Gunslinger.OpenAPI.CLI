using Tests.Constants;
using Utilities.IO;

namespace Tests.Utilities
{
	public class TestPathUtility
	{
		/// <summary>
		/// Files must be in the bin directory
		/// </summary>
		/// <param name="relativeFolder"></param>
		/// <returns></returns>
		public static string GetLocalRootPath(string relativeFolder = "")
		{
			var directoryName = FileUtility.GetBasePath<TestPathUtility>();
			return string.IsNullOrEmpty(relativeFolder)
				? directoryName
				: $"{directoryName}\\{relativeFolder}";
		}

		public static string GetTestInputDirectory(string testClassName, string testName)
		{
			return GetDirectory(testClassName, testName, TestConstants.InputDataDirectory);
		}

		public static string GetTestOutputDirectory(string testClassName, string testName)
		{
			return GetDirectory(testClassName, testName, TestConstants.OutputDataDirectory);
		}

		private static string GetDirectory(string testClassName, string testName, string root)
		{
			// className will look like this: LittlerOnDemand.Tests.Service.Implementation.TestName
			// but, that's too much - simplify it
			var className = testClassName.Split('.').ToList().Last();
			var directory = $"{root}\\{className}\\{testName}";
			return GetLocalRootPath(directory);
		}

		public static void PreparePathForUse(string path)
		{
			Directory.CreateDirectory(path);
		}

		public static string PreparePathAndFilenameForSave(string path)
		{
			var dir = Path.GetDirectoryName(path);
			PreparePathForUse(dir);
			var filename = Path.GetFileName(path);
			if (string.IsNullOrWhiteSpace(filename))
			{
				return path;
			}
			var invalids = Path.GetInvalidFileNameChars();
			var newFilename = String.Join("_", filename.Split(invalids, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
			var newpath = $"{dir}\\{newFilename}";
			return newpath;
		}

		public static IEnumerable<string> ReadAllFiles(string path, string fileExtension)
		{
			var files = Directory.GetFiles(path, $"*.{fileExtension.Replace(".", "")}");
			if (files == null || files.Length == 0)
			{
				return new List<string>();
			}
			return files.ToList();
		}
	}
}