using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OutputTests.Extensions
{
	public static class ServiceExtensions
	{
		public static void AddConfiguration<T>(this IServiceCollection services, HostBuilderContext hostContext, string configKey) where T : class
		{
			IConfigurationSection section = hostContext.Configuration.GetSection(configKey);
			string failureMessage = typeof(T).Name + " failed validation. See ServiceExtensions.AddConfiguration for details.";
			services.AddOptions<T>().Bind(section).Validate(delegate (T item)
			{
				(bool, string) tuple = ValidateObject(item);
				if (!tuple.Item1)
				{
					throw new Exception(tuple.Item2);
				}

				return tuple.Item1;
			}, failureMessage);
		}

		public static void AddOptions<T>(this IServiceCollection services, T item) where T : class
		{
			if (item == null)
			{
				throw new ArgumentNullException("Item cannot be null.");
			}

			(bool, string) tuple = ValidateObject(item);
			if (!tuple.Item1)
			{
				throw new Exception(tuple.Item2);
			}

			if (!tuple.Item1)
			{
				throw new ArgumentException("Item was not valid.");
			}

			services.AddSingleton((IServiceProvider svcProvider) => Options.Create(item));
		}

		public static T GetConfigurationSection<T>(this HostBuilderContext hostContext, string configKey)
		{
			return hostContext.Configuration.GetSection(configKey).Get<T>();
		}

		public static (bool Success, string Message) ValidateObject<T>(T item) where T : class
		{
			ValidationContext validationContext = new ValidationContext(item);
			List<ValidationResult> list = new List<ValidationResult>();
			StringBuilder stringBuilder = new StringBuilder();
			if (!Validator.TryValidateObject(item, validationContext, list, validateAllProperties: true))
			{
				Type typeFromHandle = typeof(T);
				stringBuilder.AppendLine(typeFromHandle.Name + " failed validation");
				foreach (ValidationResult item2 in list)
				{
					stringBuilder.AppendLine(item2.ErrorMessage);
				}

				return (false, stringBuilder.ToString());
			}

			return (true, string.Empty);
		}
	}
}