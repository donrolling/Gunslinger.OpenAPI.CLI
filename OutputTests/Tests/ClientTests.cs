using AutoFixture;
using OutputTests.BaseClasses;
using OutputTests.Client;
using OutputTests.Client.Models;

namespace OutputTests.Tests
{
	[TestClass]
	public class ClientTests : IntegrationTestBase
	{
		private readonly Fixture _fixture;
		private readonly ITestAPIClient _testClient;

		public ClientTests()
		{
			_fixture = new Fixture();
			_testClient = GetService<ITestAPIClient>();
		}

		/// <summary>
		/// I run these tests with the TestApiProject running in a docker container
		/// docker run -p 8080:80 -d drolling/gunslinger-api-tests:latest
		/// http://localhost:8080/swagger/index.html
		/// </summary>
		[TestMethod]
		public async Task GivenRunningClient_CreateStudentSucceeds()
		{
			var student = _fixture.Create<Student>();
			var result = await _testClient.PostStudentAsync(student);
			Assert.IsNotNull(result);
			Assert.IsTrue(result.Success, result.Message);
		}
	}
}