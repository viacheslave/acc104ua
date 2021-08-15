using acc104ua.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace acc104ua.Application
{
	public class CommandProcessor : ICommandProcessor
	{
		private readonly IAuthClient _authClient;
		private readonly IDataProvider _dataProvider;
		private readonly IDataTransformer _dataTransformer;
		private readonly IDataExporter _dataExporter;
		private readonly ILogger _logger;

		public CommandProcessor(
			IAuthClient authClient,
			IDataProvider dataProvider,
			IDataTransformer dataTransformer,
			IDataExporter dataExporter,
			ILogger logger)
		{
			_authClient = authClient;
			_dataProvider = dataProvider;
			_dataTransformer = dataTransformer;
			_dataExporter = dataExporter;
			_logger = logger;
		}

		public async Task ExportData(string[] args)
		{
			var options = GetOptions(args);

			// get raw data
			var rawData = await GetRawData(options);
			var accounts = _dataTransformer.BuildAccounts(rawData);

			await _dataExporter.SaveRaw(Utility.Gas, rawData);
			await _dataExporter.SaveRaw(Utility.Delivery, rawData);

			await _dataExporter.SaveAsJson(accounts);

			_dataExporter.SaveAsCsv(accounts);

			_dataExporter.SaveConsumptionAsCsv(accounts);

			_logger.Out($"Export folder: {_dataExporter.ExportFolder.FullName}");
		}

		public async Task<IReadOnlyCollection<AccountDataRawDto>> ShowFrontPage(string[] args)
		{
			var options = GetOptions(args);

			// get raw data
			return await GetRawData(options);
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

		private async Task<IReadOnlyCollection<AccountDataRawDto>> GetRawData(Options options)
		{
			// get auth cookies
			var authCookies = await _authClient.GetAuthCookie(
					new UserCredentials(options.Login, options.Password));

			// get data
			var data = await _dataProvider.GrabRaw(
					new Dates(options.StartDate, options.EndDate), authCookies);

			return data;
		}
	}
}
