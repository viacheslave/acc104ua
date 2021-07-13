using System;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;

namespace acc104ua
{
	internal sealed class AuthClient
	{
		/// <summary>
		///   Gets auth cookie
		/// </summary>
		/// <param name="credentials">User credentials (login/password)</param>
		/// <returns>AuthCookie value</returns>
		public async Task<AuthCookie> GetAuthCookie(UserCredentials credentials)
		{
			if (credentials is null)
				throw new ArgumentNullException(nameof(credentials));

			Logger.Out($"Logging in as {credentials.Login} ...");

			var response = await new FlurlRequest(new Url("https://ok.104.ua/ua/ajx/individual/signin/auth/credentials"))
				.PostUrlEncodedAsync(new
				{
					login = credentials.Login,
					password = credentials.Password,
				});

			return new AuthCookie
			{
				Value = response.Cookies
					.First(c => c.Name == AuthCookie.Key).Value
			};
		}
	}
}
