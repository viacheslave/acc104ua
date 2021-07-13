using System.Text.RegularExpressions;

namespace acc104ua
{
	internal static class Extensions
	{
		/// <summary>
		///		Strips money-formatted value into pure value
		/// </summary>
		/// <param name="str">Input str</param>
		/// <returns>Double-precision value</returns>
		public static double ParseMoney(this string str)
		{
			var sanitized = new Regex("грн|\\n|\b")
				.Replace(str, "")
				.Trim();

			return double.Parse(sanitized);
		}
	}
}