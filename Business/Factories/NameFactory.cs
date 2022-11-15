using Domain.Models;
using System.Text.RegularExpressions;
using Utilities.Extensions;

namespace Business.Factories
{
	public class NameFactory
	{

		public static Name Create(string name)
		{
			var scrubbedName = SafeNameFactory.Scrub(name);
			var _name = new Name
			{
				Value = name,
				LowerCamelCase = scrubbedName.ToCamelCase(),
				PascalCase = scrubbedName.ToPascalCase(),
				NameWithSpaces = name.UnCamelCase(),
				UpperCase = name.ToUpper()
			};
			var safeName = SafeNameFactory.MakeSafe(scrubbedName);
			var _safeName = new Name
			{
				Value = safeName,
				LowerCamelCase = safeName.ToCamelCase(),
				PascalCase = safeName.ToPascalCase(),
				NameWithSpaces = safeName.UnCamelCase(),
				UpperCase = safeName.ToUpper()
			};
			_name.Safe = _safeName;
			var _collisionSafe = new Name
			{
				Value = $"{_safeName.Value}_",
				LowerCamelCase = $"{_safeName.LowerCamelCase}_",
				PascalCase = $"{_safeName.PascalCase}_",
				NameWithSpaces = $"{_safeName.NameWithSpaces}_",
				UpperCase = $"{_safeName.UpperCase}_"
			};
			_name.CollisionSafe = _collisionSafe;
			return _name;
		}
	}
}