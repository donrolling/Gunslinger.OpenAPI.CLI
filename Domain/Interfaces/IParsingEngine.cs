using Domain.Configuration;
using Domain.Enums;
using Domain.Models;
using System.Text.Json;

namespace Domain.Interfaces
{
	public interface IParsingEngine
	{
		SupportedDocTypes SupportedDocTypes { get; }

		OperationResult<OpenApiResult> Parse(JsonDocument jsonDocument, GenerationContext context);
	}
}