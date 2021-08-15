using acc104ua.Application;
using acc104ua.Domain;
using System.Collections.Generic;

namespace acc104ua.Application
{
	public interface IDataTransformer
	{
		IReadOnlyCollection<AccountData> BuildAccounts(IReadOnlyCollection<AccountDataRawDto> rawData);
	}
}