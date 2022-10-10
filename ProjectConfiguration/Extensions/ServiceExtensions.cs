using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ProjectConfiguration.Extensions;

namespace ProjectConfiguration.Extensions
{
	public static class ServiceExtensions
	{
		/// <summary>
		/// Standard appSettings.json configuration acquistition code plus object validation via data annotations.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="services"></param>
		/// <param name="hostContext"></param>
		/// <param name="configKey"></param>
		/// <exception cref="Exception"></exception>
		public static void AddConfiguration<T>(
			this IServiceCollection services,
			HostBuilderContext hostContext,
			string configKey
		) where T : class
		{
			var section = hostContext.Configuration.GetSection(configKey);
			var _type = typeof(T);
			var message = $"{_type.Name} failed validation. See {nameof(ServiceExtensions)}.{nameof(AddConfiguration)} for details.";
			services.AddOptions<T>()
				.Bind(section)
				.Validate((item) =>
					{
						var validationResult = ValidationUtility.ValidateObject(item);
						if (!validationResult.Success)
						{
							throw new Exception(validationResult.Message);
						}
						return validationResult.Success;
					},
					message
				);
		}

		/// <summary>
		/// An easy way to options that you already have a class for.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="item"></param>
		public static void AddOptions<T>(this IServiceCollection services, T item) where T : class
		{
			if (item == null)
			{
				throw new ArgumentNullException("Item cannot be null.");
			}
			var validationResult = ValidationUtility.ValidateObject(item);
			if (!validationResult.Success)
			{
				throw new Exception(validationResult.Message);
			}
			if (!validationResult.Success)
			{
				throw new ArgumentException("Item was not valid.");
			}
			services.AddSingleton((svcProvider) => Options.Create(item));
		}

		public static T GetConfigurationSection<T>(this HostBuilderContext hostContext, string configKey)
		{
			return hostContext.Configuration.GetSection(configKey).Get<T>();
		}
	}
}