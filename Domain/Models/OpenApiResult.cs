﻿namespace Domain.Models
{
	public class OpenApiResult
	{
		public List<Model> Models { get; set; }

		public List<Route> Paths { get; set; }
	}
}