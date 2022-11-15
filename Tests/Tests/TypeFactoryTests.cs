using Business.Factories;
using Domain.Models;
using Tests.BaseClasses;

namespace Tests.Tests
{
	[TestClass]
	public class TypeFactoryTests : IntegrationTestBase
	{
		[TestMethod]
		public void ConfigFileTest()
		{
			var generationContext = GetGenerationContext(this, TestContext, "gunslinger.json");
			var openApiType = new OpenApiType { Type = "array", Items = new OpenApiArrayType { Type = "integer", Format = "int64" } };
			var result = TypeFactory.Create(openApiType, generationContext.TypeConfiguration);
			Assert.AreEqual("List<long>", result);
		}

		[TestMethod]
		public void VerifyArrayConversion()
		{
			var openApiType = new OpenApiType { Type = "array", Items = new OpenApiArrayType { Type = "integer", Format = "int64" } };
			var result = TypeFactory.Create(openApiType, TypeFactory.GetStandardConfiguration());
			Assert.AreEqual("List<long>", result);
		}

		[TestMethod]
		public void VerifyBooleanConversion()
		{
			var openApiType = new OpenApiType { Type = "boolean", Format = "", Nullable = false };
			var result = TypeFactory.Create(openApiType, TypeFactory.GetStandardConfiguration());
			Assert.AreEqual("bool", result);
		}

		[TestMethod]
		public void VerifyDateTimeConversion()
		{
			var openApiType = new OpenApiType { Type = "string", Format = "date-time", Nullable = false };
			var result = TypeFactory.Create(openApiType, TypeFactory.GetStandardConfiguration());
			Assert.AreEqual("DateTime", result);
		}

		[TestMethod]
		public void VerifyGuidConversion()
		{
			var openApiType = new OpenApiType { Type = "string", Format = "uuid", Nullable = false };
			var result = TypeFactory.Create(openApiType, TypeFactory.GetStandardConfiguration());
			Assert.AreEqual("Guid", result);
		}

		[TestMethod]
		public void VerifyInt64Conversion()
		{
			var openApiType = new OpenApiType { Type = "integer", Format = "int64", Nullable = false };
			var result = TypeFactory.Create(openApiType, TypeFactory.GetStandardConfiguration());
			Assert.AreEqual("long", result);
		}

		[TestMethod]
		public void VerifyIntConversion()
		{
			var openApiType = new OpenApiType { Type = "integer", Format = "int32", Nullable = false };
			var result = TypeFactory.Create(openApiType, TypeFactory.GetStandardConfiguration());
			Assert.AreEqual("int", result);
		}

		[TestMethod]
		public void VerifyFloatConversion()
		{
			var openApiType = new OpenApiType { Type = "number", Format = "float", Nullable = false };
			var result = TypeFactory.Create(openApiType, TypeFactory.GetStandardConfiguration());
			Assert.AreEqual("float", result);
		}

		[TestMethod]
		public void VerifyNullableFloatConversion()
		{
			var openApiType = new OpenApiType { Type = "number", Format = "float", Nullable = true };
			var result = TypeFactory.Create(openApiType, TypeFactory.GetStandardConfiguration());
			Assert.AreEqual("float?", result);
		}

		[TestMethod]
		public void VerifyNullableBooleanConversion()
		{
			var openApiType = new OpenApiType { Type = "boolean", Format = "", Nullable = true };
			var result = TypeFactory.Create(openApiType, TypeFactory.GetStandardConfiguration());
			Assert.AreEqual("bool?", result);
		}

		[TestMethod]
		public void VerifyNullableGuidConversion()
		{
			var openApiType = new OpenApiType { Type = "string", Format = "uuid", Nullable = true };
			var result = TypeFactory.Create(openApiType, TypeFactory.GetStandardConfiguration());
			Assert.AreEqual("Guid?", result);
		}

		[TestMethod]
		public void VerifyNullableInt64Conversion()
		{
			var openApiType = new OpenApiType { Type = "integer", Format = "int64", Nullable = true };
			var result = TypeFactory.Create(openApiType, TypeFactory.GetStandardConfiguration());
			Assert.AreEqual("long?", result);
		}
		[TestMethod]
		public void VerifyNullableIntConversion()
		{
			var openApiType = new OpenApiType { Type = "integer", Format = "int32", Nullable = true };
			var result = TypeFactory.Create(openApiType, TypeFactory.GetStandardConfiguration());
			Assert.AreEqual("int?", result);
		}
	}
}