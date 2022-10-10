using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Utilities.Extensions
{
	public static class DataAnnotationValidation
	{
		public static string GetErroredProperties(this object obj)
		{
			var sb = new StringBuilder();
			foreach (var validationResult in obj.GetValidationErrors())
			{
				sb.Append(string.Join(",", validationResult.MemberNames));
			}
			return sb.ToString();
		}

		public static string GetFullErrorMessage(this object obj)
		{
			var sb = new StringBuilder();
			foreach (var validationResult in obj.GetValidationErrors())
			{
				sb.Append(validationResult.ErrorMessage);
			}
			return sb.ToString();
		}

		public static List<ValidationResult> GetValidationErrors(this object obj)
		{
			var validationContext = new ValidationContext(obj, null, null);
			var validationResults = new List<ValidationResult>();
			Validator.TryValidateObject(obj, validationContext, validationResults, true);
			return validationResults;
		}

		public static bool IsValid(this object obj)
		{
			ValidationContext validationContext = new ValidationContext(obj, null, null);
			var resultList = new List<ValidationResult>();
			return Validator.TryValidateObject(obj, validationContext, resultList, true);
		}
	}
}