using OutputTests.Client.Models;
using System.Text.Json;

namespace OutputTests.Tests
{
	[TestClass]
	public class SerializationTests
	{
		[TestMethod]
		public void GivenAPIOutput_SerializationSucceeds()
		{
			var options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};
			var content = "{\"age\":93,\"id\":111,\"name\":\"Namefbf9d33e-7987-4925-9774-b2b42e22a734\"}";
			var result = JsonSerializer.Deserialize<Student>(content, options);
			Assert.IsNotNull(result);
			Assert.AreEqual(93, result.Age);
		}
	}
}