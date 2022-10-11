using Domain.Interfaces;
using Domain.Models;
using HandlebarsDotNet;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Reflection;
using Path = Domain.Models.Path;

namespace Engine.Engines
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

		public string Render(string template, Model model)
		{
			return Render(template, model);
		}

		public string Render(string template, ModelGroup modelGroup)
		{
			return Render(template, modelGroup);
		}

		public string Render(string template, Path path)
		{
			return Render(template, path);
		}

		private string Render<T>(string template, T obj)
		{
			try
			{

			}
			catch (Exception ex)
			{
				_logger.LogError("HandlebarsRenderEngine had a problem rendering.", ex);
				throw;
			}
			var handlebarsTemplate = Handlebars.Compile(template);
			var result = handlebarsTemplate(obj);
			return result;
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