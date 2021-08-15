using System;
using System.Linq;
using System.Threading.Tasks;
using acc104ua.Application;
using Flurl;
using Flurl.Http;

namespace acc104ua.Infrastructure
{
	internal sealed class AuthClient : IAuthClient
	{
		private readonly ILogger _logger;

		public AuthClient(ILogger logger)
		{
			_logger = logger;
		}

		/// <summary>
		///   Gets auth cookie
		/// </summary>
		/// <param name="credentials">User credentials (login/password)</param>
		/// <returns>AuthCookie value</returns>
		public async Task<AuthCookie> GetAuthCookie(UserCredentials credentials)
		{
			if (credentials is null)
				throw new ArgumentNullException(nameof(credentials));

			_logger.Out($"Logging in as {credentials.Login} ...");

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
