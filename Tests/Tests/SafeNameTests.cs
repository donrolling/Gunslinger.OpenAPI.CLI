using Business.Factories;

namespace Tests.Tests
{
	[TestClass]
	public class SafeNameTests
	{
		[TestMethod]
		public void SafePathTest()
		{
			var path = "/api/admin/serv-ice/{service-name}/contract";
			var expectedResult = "/api/admin/serv-ice/{servicename}/contract";
			var actualResult = SafeNameFactory.MakePathSafe(path);
			Assert.AreEqual(expectedResult, actualResult);
		}
	}
}