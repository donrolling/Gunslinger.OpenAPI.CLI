using System.Text.Json;
using Tests.Utilities;

namespace Tests.Extensions
{
	public static class TestFileExtensions
	{
		public static string GetTestInputPath(this TestContext testContext)
		{
			return TestPathUtility.GetTestInputDirectory(testContext.FullyQualifiedTestClassName, testContext.TestName);
		}

		public static string GetTestOutputPath(this TestContext testContext)
		{
			return TestPathUtility.GetTestOutputDirectory(testContext.FullyQualifiedTestClassName, testContext.TestName);
		}

		public static string GetTestInputFullPath(this TestContext testContext, string filename)
		{
			var inputDirectory = TestPathUtility.GetTestInputDirectory(testContext.FullyQualifiedTestClassName, testContext.TestName);
			return $"{inputDirectory}\\{filename}";
		}

		public static string GetTestOutputFullPath(this TestContext testContext, string filename)
		{
			var inputDirectory = TestPathUtility.GetTestOutputDirectory(testContext.FullyQualifiedTestClassName, testContext.TestName);
			return $"{inputDirectory}\\{filename}";
		}

		public static Stream ReadFileAsStream(this TestContext testContext, string filename)
		{
			return TestIOUtility.ReadFileAsStream(testContext.GetTestInputFullPath(filename));
		}

		public static string ReadAllText(this TestContext testContext, string filename)
		{
			return TestIOUtility.ReadAllText(testContext.GetTestInputFullPath(filename));
		}

		public static T ReadJSON<T>(this TestContext testContext, string filename)
		{
			var result = TestIOUtility.ReadAllText(testContext.GetTestInputFullPath(filename));
			return JsonSerializer.Deserialize<T>(result);
		}
	}
}