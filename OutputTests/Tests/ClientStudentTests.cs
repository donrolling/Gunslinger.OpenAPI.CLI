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
	public class ClientStudentTests : IntegrationTestBase
	{
		private readonly Fixture _fixture;
		private readonly ITestAPIClient _testClient;

		public ClientStudentTests()
		{
			_fixture = new Fixture();
			_testClient = GetService<ITestAPIClient>();
		}

		[TestMethod]
		public async Task GivenRunningClient_CreateStudentSucceeds()
		{
			var student = _fixture.Create<Student>();
			var result = await _testClient.PostStudentAsync(student);
			Assert.IsNotNull(result);
			Assert.IsTrue(result.Success, result.Message);
			Assert.AreEqual(student.Age, result.Result.Age);
		}

		[TestMethod]
		public async Task GivenRunningClient_DeleteStudentSucceeds()
		{
			var result = await _testClient.DeleteStudentAsync(10);
			Assert.IsNotNull(result);
			Assert.IsTrue(result.Success, result.Message);
		}

		[TestMethod]
		public async Task GivenRunningClient_GetStudentSucceeds()
		{
			var result = await _testClient.GetStudentAsync(10);
			Assert.IsNotNull(result);
			Assert.IsTrue(result.Success, result.Message);
			Assert.AreEqual(10, result.Result.Id);
		}

		[TestMethod]
		public async Task GivenRunningClient_PutStudentSucceeds()
		{
			var student = _fixture.Create<Student>();
			var result = await _testClient.PutStudentAsync(student);
			Assert.IsNotNull(result);
			Assert.IsTrue(result.Success, result.Message);
			Assert.AreEqual(student.Age, result.Result.Age);
		}
	}
}