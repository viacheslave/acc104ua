using System.Text.RegularExpressions;

namespace acc104ua
{
	internal static class Extensions
	{
		/// <summary>
		///		Converts raw strings into double values
		/// </summary>
		/// <param name="str">Input str</param>
		/// <returns>Double-precision value</returns>
		public static double Parse(this string str)
		{
			var sanitized = Strip(str);

			return double.Parse(sanitized);
		}

		/// <summary>
		///		Sanitizes raw strings
		/// </summary>
		/// <param name="str">Input str</param>
		/// <returns>Sanitized str</returns>
		public static string Strip(this string str)
		{
			str = new Regex("грн|\\n|\b|м³|/")
				.Replace(str, "")
				.Trim();

			return new Regex("\\s\\s+")
				.Replace(str, " ");
		}
	}
}