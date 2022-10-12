using Domain;
using Domain.Interfaces;
using HandlebarsDotNet;
using Microsoft.Extensions.Logging;

namespace Business.Engines
{
	public class HandlebarsRenderEngine : IRenderEngine
	{
		private readonly ILogger<HandlebarsRenderEngine> _logger;

		static HandlebarsRenderEngine()
		{
			RegisterHelpers();
		}

		public HandlebarsRenderEngine(ILogger<HandlebarsRenderEngine> logger)
		{
			_logger = logger;
		}

		public OperationResult<string> Render(string template, IRenderable item)
		{
			try
			{
				var handlebarsTemplate = Handlebars.Compile(template);
				var result = handlebarsTemplate(item);
				return OperationResult.Ok(result);
			}
			catch (Exception ex)
			{
				var message = $"HandlebarsRenderEngine had a problem rendering.\r\nException: {ex.Message}";
				_logger.LogError(message);
				return OperationResult.Ok(message);
			}
		}

		private static void RegisterHelpers()
		{
			// register an ifCond helper so I can write if statements in the templates
			Handlebars.RegisterHelper("ifCond", (writer, options, context, arguments) =>
			{
				var type0 = arguments[0].GetType().Name;
				var type1 = arguments[1].GetType().Name;
				var argument0 = arguments[0];
				var argument1 = arguments[1];
				if (type0 == "UndefinedBindingResult")
				{
					var message = $"argument0: {argument0} - argument1: {argument1}";
					throw new Exception($"Argument 0 undefined. Detail: {message}");
				}
				if (type1 == "UndefinedBindingResult")
				{
					var message = $"argument0: {argument0} - argument1: {argument1}";
					throw new Exception($"Argument 1 undefined. Detail: {message}");
				}
				if (type0 == "string" && type1 == "string")
				{
					if (argument0.Equals(argument1))
					{
						options.Template(writer, (object)context);
					}
					else
					{
						options.Inverse(writer, (object)context);
					}
				}
				else
				{
					if (argument0 == argument1)
					{
						options.Template(writer, (object)context);
					}
					else
					{
						options.Inverse(writer, (object)context);
					}
				}
			});
			// register an ifCond helper so I can write if statements in the templates
			Handlebars.RegisterHelper("boolCond", (writer, options, context, arguments) =>
			{
				var arguments0 = arguments[0];
				if ((bool)arguments0)
				{
					options.Template(writer, (object)context);
				}
				else
				{
					options.Inverse(writer, (object)context);
				}
			});
		}
	}
}