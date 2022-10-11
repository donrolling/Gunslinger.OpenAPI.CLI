﻿namespace Domain.Models
{
	public class Model
	{
		public Name Name { get; set; }

		public string Namespace { get; set; }

		public List<Property> Properties { get; set; } = new List<Property>();

		public string TypeName { get; set; }
	}
}