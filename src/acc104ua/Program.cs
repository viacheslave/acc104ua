using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace acc104ua
{
	static class Program
	{
		async static Task Main(string[] args)
		{
			Console.OutputEncoding = System.Text.Encoding.UTF8;

			await ProcessMode(args[0], args);

			Console.WriteLine("All Done.");
		}

		private static async Task ProcessMode(string option, string[] args)
		{
			switch (option)
			{
				case "--export":
					await ExportData(args.Skip(1).ToArray());
					break;
				default:
					break;
			}
		}

		private static async Task ExportData(string[] args)
		{
			var options = GetOptions(args);

			// get raw data
			var rawData = await GetRawData(options);
			var accounts = new DataTransformer().BuildAccounts(rawData);

			var exporter = new DataExporter();

			await exporter.SaveRaw(Utility.Gas, rawData);
			await exporter.SaveRaw(Utility.Delivery, rawData);

			await exporter.SaveAsJson(accounts);

			exporter.SaveAsCsv(accounts);

			exporter.SaveConsumptionAsCsv(accounts);

			Logger.Out($"Export folder: {exporter.ExportFolder.FullName}");
		}

		private static Options GetOptions(string[] args)
		{
			var dateDefault = DateTime.Now.Date.AddMonths(-1);

			var options = new Options(args[0], args[1], dateDefault, dateDefault);

			if (args.Length >= 3)
				options = options with { StartDate = DateTime.Parse(args[2]) };

			if (args.Length >= 4)
				options = options with { EndDate = DateTime.Parse(args[3]) };

			return options;
		}

		private static async Task<IReadOnlyCollection<AccountDataRawDto>> GetRawData(Options options)
		{
			// get auth cookies
			var authCookies = await new AuthClient()
				.GetAuthCookie(
					new UserCredentials(options.Login, options.Password));

			// get data
			var data = await new DataProvider(authCookies)
				.GrabRaw(
					new Dates(options.StartDate, options.EndDate));

			return data;
		}

		private record Options
		(
			string Login,
			string Password,
			DateTime StartDate,
			DateTime EndDate
		);
	}
}
