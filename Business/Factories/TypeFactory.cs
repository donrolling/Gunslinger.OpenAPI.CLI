using Domain.Models;

namespace Business.Factories
{
	public static class TypeFactory
	{
		public static string Create(OpenApiType type, List<DataTypeTransformation> dataTypeTransformations)
		{
			var result = FindType(type, dataTypeTransformations);
			if (result != null)
			{
				return result.DestinationType;
			}
			return string.IsNullOrEmpty(type.Format) ? type.Type : type.Format;
		}

		private static DataTypeTransformation FindType(OpenApiType type, List<DataTypeTransformation> dataTypeTransformations)
		{
			if (type.Type == "array")
			{
				return FindArrayType(type, dataTypeTransformations);
			}
			var result = dataTypeTransformations.FirstOrDefault(a =>
				a.DataType == type.Type
				&& a.Format == type.Format
				&& a.Nullable == type.Nullable
			);
			return result;
		}

		/// <summary>
		/// Warning! This currently only handles array of depth = 1
		/// I didn't have any good examples of how to handle more than that, so I quit.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="dataTypeTransformations"></param>
		/// <returns></returns>
		private static DataTypeTransformation FindArrayType(OpenApiType type, List<DataTypeTransformation> dataTypeTransformations)
		{
			if (type.Items == null)
			{
				return null;
			}
			var result = dataTypeTransformations.FirstOrDefault(a =>
				a.DataType == "array"
				&& a.Items.DataType == type.Items.Type
				&& a.Format == type.Format
				&& a.Nullable == type.Nullable
			);
			return result;
		}

		public static List<DataTypeTransformation> GetStandardConfiguration()
		{
			return new List<DataTypeTransformation>
			{
				new DataTypeTransformation
				{
					DataType = "boolean",
					Format = "",
					DestinationType = "bool"
				},
				new DataTypeTransformation
				{
					DataType = "boolean",
					Format = "",
					Nullable = true,
					DestinationType = "bool?"
				},
				new DataTypeTransformation
				{
					DataType = "string",
					Format = "date",
					DestinationType = "DateTime"
				},
				new DataTypeTransformation{
					DataType = "string",
					Format = "date-time",
					DestinationType = "DateTime"
				},
				new DataTypeTransformation {
					DataType = "string",
					Format = "password",
					DestinationType = "string"
				},
				new DataTypeTransformation {
					DataType = "string",
					Format = "byte",
					DestinationType = "Byte"
				},
				new DataTypeTransformation {
					DataType = "string",
					Format = "date",
					DestinationType = "DateTime"
				},
				new DataTypeTransformation {
					DataType = "string",
					Format = "binary",
					DestinationType = "string"
				},
				new DataTypeTransformation {
					DataType = "string",
					Format = "email",
					DestinationType = "string"
				},
				new DataTypeTransformation {
					DataType = "string",
					Format = "uuid",
					DestinationType = "Guid"
				},
				new DataTypeTransformation {
					DataType = "string",
					Format = "uuid",
					Nullable = true,
					DestinationType = "Guid?"
				},
				new DataTypeTransformation {
					DataType = "string",
					Format = "uri",
					DestinationType = "Uri"
				},
				new DataTypeTransformation {
					DataType = "number",
					Format = "-",
					DestinationType = "int"
				},
				new DataTypeTransformation {
					DataType = "number",
					Format = "double",
					DestinationType = "Double"
				},
				new DataTypeTransformation {
					DataType = "integer",
					Format = "int32",
					DestinationType = "int"
				},
				new DataTypeTransformation {
					DataType = "integer",
					Format = "int64",
					DestinationType = "long"
				},
				new DataTypeTransformation {
					DataType = "integer",
					Format = "int16",
					Nullable = true,
					DestinationType = "int?"
				},
				new DataTypeTransformation {
					DataType = "integer",
					Format = "int16",
					Nullable = false,
					DestinationType = "int"
				},
				new DataTypeTransformation {
					DataType = "number",
					Format = "float",
					Nullable = true,
					DestinationType = "float?"
				},
				new DataTypeTransformation {
					DataType = "number",
					Format = "float",
					Nullable = false,
					DestinationType = "float"
				},
				new DataTypeTransformation {
					DataType = "integer",
					Format = "int32",
					Nullable = true,
					DestinationType = "int?"
				},
				new DataTypeTransformation {
					DataType = "integer",
					Format = "int64",
					Nullable = true,
					DestinationType = "long?"
				},
				new DataTypeTransformation {
					DataType = "array",
					Items = new DataTypeTransformation {
						DataType = "integer",
						Format = "int64"
					},
					DestinationType = "List<long>"
				}
			};
		}
	}
}