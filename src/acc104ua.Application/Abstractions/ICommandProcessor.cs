using System.Collections.Generic;
using System.Threading.Tasks;

namespace acc104ua.Application
{
	public interface ICommandProcessor
	{
		Task ExportData(string[] args);

		Task<IReadOnlyCollection<AccountDataRawDto>> ShowFrontPage(string[] args);
	}
}