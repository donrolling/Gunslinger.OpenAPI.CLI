using Tests.BaseClasses;

namespace Tests.Tests
{
	[TestClass]
	public class SwaggerReadTests : IntegrationTestBase
	{
		public SwaggerReadTests()
		{
		}

		[TestMethod]
		public async Task ReadFromConfiguration()
		{
			var result = await RunGeneratorAsync(this, TestContext, "gunslinger.json");
			Assert.IsTrue(result.Success, result.Message);
		}
	}
}