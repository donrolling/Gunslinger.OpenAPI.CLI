using AutoFixture;
using OutputTests.BaseClasses;
using OutputTests.Client;
using OutputTests.Client.Models;

namespace OutputTests.Tests
{
	/// <summary>
	/// I run these tests with the TestApiProject running in a docker container
	/// docker run -p 8080:80 -d drolling/gunslinger-api-tests:latest
	/// http://localhost:8080/swagger/index.html
	/// </summary>
	[TestClass]
	public class ClientCourseTests : IntegrationTestBase
	{
		private readonly Fixture _fixture;
		private readonly ITestAPIClient _testClient;

		public ClientCourseTests()
		{
			_fixture = new Fixture();
			_testClient = GetService<ITestAPIClient>();
		}

		[TestMethod]
		public async Task GivenRunningClient_CreateCourseSucceeds()
		{
			var course = _fixture.Create<Course>();
			var result = await _testClient.PostCourseAsync(course);
			Assert.IsNotNull(result);
			Assert.IsTrue(result.Success, result.Message);
			Assert.AreEqual(course.Number, result.Result.Number);
			Assert.AreEqual(course.Name, result.Result.Name);
		}

		[TestMethod]
		public async Task GivenRunningClient_DeleteCourseSucceeds()
		{
			var result = await _testClient.DeleteCourseAsync(10);
			Assert.IsNotNull(result);
			Assert.IsTrue(result.Success, result.Message);
		}

		[TestMethod]
		public async Task GivenRunningClient_GetCourseSucceeds()
		{
			var result = await _testClient.GetCourseAsync(10);
			Assert.IsNotNull(result);
			Assert.IsTrue(result.Success, result.Message);
			Assert.AreEqual(10, result.Result.Id);
		}

		[TestMethod]
		public async Task GivenRunningClient_PutCourseSucceeds()
		{
			var course = _fixture.Create<Course>();
			var result = await _testClient.PutCourseAsync(course);
			Assert.IsNotNull(result);
			Assert.IsTrue(result.Success, result.Message);
			Assert.AreEqual(course.Number, result.Result.Number);
			Assert.AreEqual(course.Name, result.Result.Name);
		}
	}
}