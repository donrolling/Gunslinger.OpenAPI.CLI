using System.ComponentModel.DataAnnotations;

namespace Utilities.Validation
{
	public static class DataAnnotationValidation
	{
		public static bool IsValid(this object obj)
		{
			var validationContext = new ValidationContext(obj, null, null);
			var resultList = new List<ValidationResult>();
			return Validator.TryValidateObject(obj, validationContext, resultList, true);
		}
	}
}