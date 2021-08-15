namespace acc104ua.Application
{
	public sealed class AuthCookie
	{
		public const string Key = "PHPSESSID";

		public string Value { get; init; }
	}
}
