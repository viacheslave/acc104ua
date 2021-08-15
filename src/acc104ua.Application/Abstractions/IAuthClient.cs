using acc104ua.Application;
using System.Threading.Tasks;

namespace acc104ua.Application
{
	public interface IAuthClient
	{
		Task<AuthCookie> GetAuthCookie(UserCredentials credentials);
	}
}