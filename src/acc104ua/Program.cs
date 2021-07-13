﻿using System;
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

			await ExportData(options, Utility.Gas);
			await ExportData(options, Utility.Delivery);
		}

		private static async Task ExportData(Options options, Utility utility)
		{
			// get raw data
			var rawData = await GetRawData(options, utility);

			var accounts = new DataTransformer().BuildAccounts(rawData);

			var exporter = new DataExporter();

			// export raw data
			await exporter.SaveRaw(utility, rawData);

			// export raw data
			await exporter.SaveAsJson(utility, accounts);

			// export CSV data
			exporter.SaveAsCsv(utility, accounts);
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

		private static async Task<IReadOnlyCollection<AccountDataRawDto>> GetRawData(Options options, Utility utility)
		{
			// get auth cookies
			var authCookies = await new AuthClient()
				.GetAuthCookie(
					new UserCredentials(options.Login, options.Password));

			// get data
			var data = await new DataProvider(authCookies)
				.GrabRaw(
					utility,
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
