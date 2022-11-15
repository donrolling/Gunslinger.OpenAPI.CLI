using Business.Factories;

namespace Tests.Tests
{
	[TestClass]
	public class NameFactoryTests
	{
		public NameFactoryTests()
		{
		}

		[TestMethod]
		public void DashTest()
		{
			var input = "My-Name";
			var expectedResult = "MyName";
			var result = NameFactory.Create(input);
			Assert.AreEqual(expectedResult, result.Safe.Value);
		}
	}
}