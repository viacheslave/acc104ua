using System.Collections.Generic;
using System.Threading.Tasks;

namespace acc104ua.Application
{
	public interface IDataProvider
	{
		Task<IReadOnlyCollection<AccountDataRawDto>> GrabRaw(Dates dates, AuthCookie authCookie);
	}
}