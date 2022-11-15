using System.Text.RegularExpressions;

namespace Business.Factories
{
	public static class SafeNameFactory
	{
		private static List<string> _reservedNames = new List<string>();

		private static Regex nameScrubPattern = new Regex("[^a-zA-Z0-9 -]");

		private static Regex safeNamePattern = new Regex("[-]");
		
		private static Regex safePathPattern = new Regex(@"\{\w*-\w*\}");

		public static string MakeSafe(string name)
		{
			var safeName = safeNamePattern.Replace(name, "");
			// needs to be case-sensitive
			if (_reservedNames.Contains(safeName))
			{
				safeName += "_";
			}
			return safeName;
		}

		public static string Scrub(string name)
		{
			return nameScrubPattern.Replace(name, "");
		}

		public static string ScrubAndMakeSafe(string name)
		{
			return MakeSafe(Scrub(name));
		}

		public static string MakeTypeSafe(string name)
		{
			return MakeSafe(name);
		}

		public static string MakePathSafe(string path)
		{
			foreach (Match match in safePathPattern.Matches(path))
			{
				path = path.Replace(match.Value, match.Value.Replace("-", ""));
			}			
			return path;
		}

		public static void SetReservedNames(List<string> reservedNames)
		{
			if (reservedNames != null && reservedNames.Any())
			{
				_reservedNames = reservedNames;
			}
			_reservedNames.Add("event");
		}
	}
}