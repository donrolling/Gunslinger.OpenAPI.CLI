namespace Business.Factories
{
	public static class TypeFactory
	{
		public static string Create(string type, string typeFormat)
		{
			var _type = string.IsNullOrWhiteSpace(typeFormat) ? type : typeFormat;
			return ConvertToCSharpType(_type);
		}

		private static string ConvertToCSharpType(string type)
		{
			if (type == "int32")
			{
				return "int";
			}
			else if (type == "int64")
			{
				return "long";
			}
			else if (type == "boolean")
			{
				return "bool";
			}
			else if (type.StartsWith("int"))
			{
				return "int";
			}
			// no change needed, or no known transform created
			return type;
		}
	}
}