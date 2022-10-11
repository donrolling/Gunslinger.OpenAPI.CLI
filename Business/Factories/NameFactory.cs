using Domain.Models;
using System.Text.RegularExpressions;
using Utilities.Extensions;

namespace Business.Factories
{
	public class NameFactory
	{
		public static Regex rgx = new Regex("[^a-zA-Z0-9 -]");

		public static Name Create(string name)
		{
			return new Name
			{
				Value = name,
				LowerCamelCase = rgx.Replace(name.ToCamelCase(), ""),
				PascalCase = rgx.Replace(name.ToPascalCase(), ""),
				NameWithSpaces = name.UnCamelCase(),
				UpperCase = name.ToUpper()
			};
		}
	}
}