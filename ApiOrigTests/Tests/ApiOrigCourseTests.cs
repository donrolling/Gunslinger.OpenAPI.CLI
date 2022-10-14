using ApiOrigTests.Client;
using AutoFixture;
using IntegrationTestProject.BaseClasses;

namespace OutputTests.Tests
{
	[TestClass]
	public class ApiOrigCourseTests : IntegrationTestBase
	{
		private readonly Fixture _fixture;
		private readonly IApiOrigClient _testClient;

		public ApiOrigCourseTests()
		{
			_fixture = new Fixture();
			_testClient = GetService<IApiOrigClient>();
		}

		[TestMethod]
		public async Task GivenRunningClient_CreateCourseSucceeds()
		{
			var hoursOld = 1;
			var affiliate = true;
			var result = await _testClient.PostOriginationGetChaseYetToFundEmailCandidatesAndEmailAsync(hoursOld, affiliate);
			Assert.IsNotNull(result);
			Assert.IsTrue(result.Success, result.Message);
		}
	}
}