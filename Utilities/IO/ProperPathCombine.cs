﻿using System.Text;

namespace Utilities.IO
{
	public static class ProperPathCombine
	{
		public static string Combine(List<string> parts)
		{
			var sb = new StringBuilder();
			var first = true;
			foreach (var part in parts)
			{
				if (first)
				{
					sb.Append(part);
				}
				else
				{
					sb.Append("\\");
					sb.Append(part.Trim('\\'));
				}
				first = false;
			}
			return sb.ToString();
		}
	}
}