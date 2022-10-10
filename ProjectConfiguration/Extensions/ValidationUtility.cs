using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProjectConfiguration.Extensions
{
	public static class ValidationUtility
	{
		public static (bool Success, string Message) ValidateObject<T>(T item) where T : class
		{
			var ctx = new ValidationContext(item);
			var validationResults = new List<ValidationResult>();
			var message = new StringBuilder();
			if (!Validator.TryValidateObject(item, ctx, validationResults, true))
			{
				var _type = typeof(T);
				message.AppendLine($"{_type.Name} failed validation");
				foreach (var validationResult in validationResults)
				{
					message.AppendLine(validationResult.ErrorMessage);
				}
				return (false, message.ToString());
			}
			return (true, string.Empty);
		}
	}
}