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
		public async Task OutputTest()
		{
			var result = await RunGeneratorAsync(this, TestContext, "gunslinger.json");
			Assert.IsTrue(result.Success, result.Message);
		}

		[TestMethod]
		public async Task RunGeneratorAgainstStoredJson()
		{
			var result = await RunGeneratorAsync(this, TestContext, "gunslinger.json");
			Assert.IsTrue(result.Success, result.Message);
		}

		[TestMethod]
		public async Task RunGeneratorAgainst2_0Json()
		{
			var result = await RunGeneratorAsync(this, TestContext, "gunslinger.json");
			Assert.IsTrue(result.Success, result.Message);
		}

		[TestMethod]
		public async Task RunGeneratorAgainst2_0Json_SecurePayments()
		{
			var result = await RunGeneratorAsync(this, TestContext, "gunslinger.json");
			Assert.IsTrue(result.Success, result.Message);
		}
	}
}