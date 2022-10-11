using System.Text.RegularExpressions;
using Tests.Constants;

namespace Tests.Utilities
{
	public static class TestIOUtility
	{
		private static string _currentDirectory;

		private static string _currentDirectoryNoBin;

		public static string CurrentDirectory
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_currentDirectory))
				{
					_currentDirectory = Environment.CurrentDirectory;
				}
				return _currentDirectory;
			}
		}

		public static string CurrentDirectoryNoBin
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_currentDirectoryNoBin))
				{
					_currentDirectoryNoBin = Regex.Split(Environment.CurrentDirectory, @"\\bin\\")[0];
				}
				return _currentDirectoryNoBin;
			}
		}

		public static void DeleteFile(string path)
		{
			File.Delete(path);
		}

		public static bool Exists(string path)
		{
			return File.Exists(path);
		}

		public static string GetProjectDirectory(string subdirectory = "", bool removeBinFolder = true)
		{
			var result = removeBinFolder ? CurrentDirectoryNoBin : CurrentDirectory;
			return string.IsNullOrWhiteSpace(subdirectory)
				? result
				: $"{result}\\{subdirectory}";
		}

		public static string GetTestInputDirectory(string subdirectory = "")
		{
			return string.IsNullOrWhiteSpace(subdirectory)
				? $"{CurrentDirectoryNoBin}\\{TestConstants.InputDataDirectory}"
				: $"{CurrentDirectoryNoBin}\\{TestConstants.InputDataDirectory}\\{subdirectory}";
		}

		public static string GetTestOutputDirectory(string subdirectory = "")
		{
			return string.IsNullOrWhiteSpace(subdirectory)
				? $"{CurrentDirectoryNoBin}\\{TestConstants.InputDataDirectory}"
				: $"{CurrentDirectoryNoBin}\\{TestConstants.InputDataDirectory}\\{subdirectory}";
		}

		public static string ReadAllText(string path)
		{
			if (!File.Exists(path)) { return string.Empty; }
			return File.ReadAllText(path);
		}

		public static byte[] ReadFile(string path)
		{
			if (!File.Exists(path)) { return null; }
			return File.ReadAllBytes(path);
		}

		public static Stream ReadFileAsStream(string path)
		{
			if (!File.Exists(path)) { return null; }
			return File.OpenRead(path);
		}

		public static void RenameFile(string path, string newPath)
		{
			File.Move(path, newPath);
		}

		public static void SaveFile(string path, byte[] content)
		{
			var newpath = TestPathUtility.PreparePathAndFilenameForSave(path);
			File.WriteAllBytes(newpath, content);
		}

		public static void SaveFile(string path, string json)
		{
			var newpath = TestPathUtility.PreparePathAndFilenameForSave(path);
			File.WriteAllText(newpath, json);
		}
	}
}