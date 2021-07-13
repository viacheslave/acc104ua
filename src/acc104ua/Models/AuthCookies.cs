namespace acc104ua
{
	internal sealed class AuthCookie
	{
		public const string Key = "PHPSESSID";

		public string Value { get; init; }
	}
}
