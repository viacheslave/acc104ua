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
		private DirectoryInfo _exportFolder;

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
					_exportFolder.FullName, 
					$"raw-acc{account.AccountId}-{fnSuffix}.txt");

				await File.WriteAllTextAsync(fileName, string.Concat(account.Lines));

				Logger.Out($"Raw: {fileName}");
			}

			Logger.Out($"Raw export folder: {_exportFolder.FullName}");
		}

		/// <summary>
		///		Exports raw data as JSON
		/// </summary>
		/// <param name="utility">Gas or Delivery</param>
		/// <param name="accounts">Accounts data</param>
		public async Task SaveAsJson(Utility utility, IReadOnlyCollection<AccountData> accounts)
		{
			var fnSuffix = GetFilenameSuffix(utility);

			Logger.Out($"Exporting JSON data...");

			foreach (var account in accounts)
			{
				var fileName = Path.Combine(
					_exportFolder.FullName,
					$"acc{account.AccountId}-{fnSuffix}.json");

				var json = JsonSerializer.Serialize(account, _serializerOptions);

				await File.WriteAllTextAsync(fileName, json);

				Logger.Out($"JSON: {fileName}");
			}

			Logger.Out($"JSON export folder: {_exportFolder.FullName}");
		}

		/// <summary>
		///		Exports raw data as CSV
		/// </summary>
		/// <param name="utility">Gas or Delivery</param>
		/// <param name="accounts">Accounts data</param>
		public void SaveAsCsv(Utility utility, IReadOnlyCollection<AccountData> accounts)
		{
			var fnSuffix = GetFilenameSuffix(utility);

			var csvConfiguration = new CsvConfiguration(CultureInfo.GetCultureInfo("uk-UA"));

			Logger.Out($"Exporting CSV data...");

			foreach (var account in accounts)
			{
				var accountId = account.AccountId;

				var exported = account.Lines
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

				var fileName = Path.Combine(
					_exportFolder.FullName,
					$"acc{accountId}-{fnSuffix}.csv");

				using var tw = new StreamWriter(fileName);
				using var csv = new CsvWriter(tw, csvConfiguration);

				csv.Context.RegisterClassMap<MonthlyLineExportDtoMap>();
				csv.WriteRecords(exported);
			}

			Logger.Out($"CSV export folder: {_exportFolder.FullName}");
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

			_exportFolder = new DirectoryInfo(folder);

			if (!Directory.Exists(folder))
				_exportFolder = Directory.CreateDirectory(folder);
		}
	}
}
