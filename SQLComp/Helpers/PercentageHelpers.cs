namespace SQLComp.Helpers
{
	public static class PercentageHelpers
	{
		public static string GetPercentage(uint current, uint max)
		{
			if (current == 0)
				return "0%";
			if (max == 0)
				return "?%";
			return $"{Math.Round(((decimal)current / (decimal)max) * 100, 2)}%";
		}

		public static string GetPercentage(uint current, int max)
		{
			if (current == 0)
				return "0%";
			if (max == 0)
				return "?%";
			return $"{Math.Round(((decimal)current / (decimal)max) * 100, 2)}%";
		}
	}
}
