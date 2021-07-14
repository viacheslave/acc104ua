using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;

namespace acc104ua
{
	internal sealed class DataExporter
	{
		public DirectoryInfo ExportFolder { get; private set; }

		private readonly JsonSerializerOptions _serializerOptions = new()
		{
			WriteIndented = true,
			Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
		};

		public DataExporter()
		{
			EnsureExportFolder();
		}

		/// <summary>
		///		Exports raw data as TXT (HTML block)
		/// </summary>
		/// <param name="utility">Gas or Delivery</param>
		/// <param name="accounts">Accounts data</param>
		public async Task SaveRaw(Utility utility, IReadOnlyCollection<AccountDataRawDto> accounts)
		{
			var fnSuffix = GetFilenameSuffix(utility);

			Logger.Out($"Exporting raw data...");

			foreach (var account in accounts)
			{
				var fileName = Path.Combine(
					ExportFolder.FullName, 
					$"raw-acc{account.AccountId}-{fnSuffix}.txt");

				var data = utility switch
				{
					Utility.Delivery => account.DeliveryLines,
					Utility.Gas => account.GasLines
				};

				await File.WriteAllTextAsync(fileName, string.Concat(data));

				Logger.Out($"Raw: {fileName}");
			}
		}

		/// <summary>
		///		Exports raw data as JSON
		/// </summary>
		/// <param name="accounts">Accounts data</param>
		public async Task SaveAsJson(IReadOnlyCollection<AccountData> accounts)
		{
			Logger.Out($"Exporting JSON data...");

			foreach (var account in accounts)
			{
				var fileName = Path.Combine(ExportFolder.FullName, $"acc{account.AccountId}.json");
				var json = JsonSerializer.Serialize(account, _serializerOptions);

				await File.WriteAllTextAsync(fileName, json);

				Logger.Out($"JSON: {fileName}");
			}
		}

		/// <summary>
		///		Exports data as CSV
		/// </summary>
		/// <param name="accounts">Accounts data</param>
		public void SaveAsCsv(IReadOnlyCollection<AccountData> accounts)
		{
			var csvConfiguration = new CsvConfiguration(CultureInfo.GetCultureInfo("uk-UA"));

			Logger.Out($"Exporting CSV data...");

			foreach (var account in accounts)
			{
				var fnSuffix = GetFilenameSuffix(Utility.Gas);
				var fileName = Path.Combine(ExportFolder.FullName, $"acc{account.AccountId}-{fnSuffix}.csv");
				ExportLines(account, account.GasLines, fileName);

				fnSuffix = GetFilenameSuffix(Utility.Delivery);
				fileName = Path.Combine(ExportFolder.FullName, $"acc{account.AccountId}-{fnSuffix}.csv");
				ExportLines(account, account.DeliveryLines, fileName);

				Logger.Out($"CSV: {fileName}");
			}

			void ExportLines(AccountData acc, IReadOnlyCollection<MonthlyLine> lines, string fileName)
			{
				var accountId = acc.AccountId;

				var linesDtos = lines
					.Select(model => new MonthlyLineExportDto
					{
						AccountId = accountId,
						Month = model.Month,
						BalancePeriodStart = model.BalancePeriodStart,
						Paid = model.Paid,
						Subs = model.Subs,
						Credited = model.Credited,
						Recalculated = model.Recalculated,
						Penalty = model.Penalty,
						BalancePeriodEnd = model.BalancePeriodEnd
					})
					.ToList();

				using var tw = new StreamWriter(fileName);
				using var csv = new CsvWriter(tw, csvConfiguration);

				csv.Context.RegisterClassMap<MonthlyLineExportDtoMap>();
				csv.WriteRecords(linesDtos);
			}
		}

		/// <summary>
		///		Export consumption data as CSV
		/// </summary>
		/// <param name="accounts">Accounts data</param>
		public void SaveConsumptionAsCsv(IReadOnlyCollection<AccountData> accounts)
		{
			var csvConfiguration = new CsvConfiguration(CultureInfo.GetCultureInfo("uk-UA"));

			Logger.Out($"Exporting CSV consumption data...");

			foreach (var account in accounts)
			{
				var fileName = Path.Combine(ExportFolder.FullName, $"acc{account.AccountId}-consumption.csv");
				ExportConsumption(account, fileName);

				Logger.Out($"CSV: {fileName}");
			}

			void ExportConsumption(AccountData acc, string fileName)
			{
				var accountId = acc.AccountId;

				var linesDtos = acc.Consumption
					.Select(model => new MonthlyConsumptionExportDto
					{
						Period = DateTime.Parse(model.Period),
						Power = model.Power,
						Volume = model.Volume
					})
					.ToList();

				using var tw = new StreamWriter(fileName);
				using var csv = new CsvWriter(tw, csvConfiguration);

				csv.Context.RegisterClassMap<MonthlyConsumptionExportDtoMap>();
				csv.WriteRecords(linesDtos);
			}
		}

		private static string GetFilenameSuffix(Utility utility)
		{
			return utility switch
			{
				Utility.Delivery => "delivery",
				Utility.Gas => "gas",
				_ => throw new NotImplementedException()
			};
		}

		private void EnsureExportFolder()
		{
			var folder = $"export-{DateTime.Now:yyyyMMdd-HHmm}";

			ExportFolder = new DirectoryInfo(folder);

			if (!Directory.Exists(folder))
				ExportFolder = Directory.CreateDirectory(folder);
		}

	}
}
