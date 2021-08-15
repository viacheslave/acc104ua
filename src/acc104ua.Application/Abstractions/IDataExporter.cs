using acc104ua.Application;
using acc104ua.Domain;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace acc104ua.Application
{
	public interface IDataExporter
	{
		DirectoryInfo ExportFolder { get; }

		void SaveAsCsv(IReadOnlyCollection<AccountData> accounts);
		Task SaveAsJson(IReadOnlyCollection<AccountData> accounts);
		void SaveConsumptionAsCsv(IReadOnlyCollection<AccountData> accounts);
		Task SaveRaw(Utility utility, IReadOnlyCollection<AccountDataRawDto> accounts);
	}
}