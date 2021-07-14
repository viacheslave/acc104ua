using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace acc104ua
{
	internal class DataTransformer
	{
		private static readonly CultureInfo _cultureInfo = CultureInfo.GetCultureInfo("uk-UA");

		/// <summary>
		///		Builds accounts data
		/// </summary>
		/// <param name="rawData">Raw data (HTML)</param>
		/// <returns>Collection of accounts</returns>
		internal IReadOnlyCollection<AccountData> BuildAccounts(IReadOnlyCollection<AccountDataRawDto> rawData)
		{
			var accounts = new List<AccountData>();

			foreach (var rawDataItem in rawData)
			{
				var gasLines = GetLines(rawDataItem.GasLines);
				var deliveryLines = GetLines(rawDataItem.DeliveryLines);

				var consumption = new List<MonthlyConsumption>();
				for (var i = 0; i < rawDataItem.Consumption.data.periods.Length; ++i)
				{
					consumption.Add(new MonthlyConsumption(
						rawDataItem.Consumption.data.volumes[i],
						rawDataItem.Consumption.data.power[i],
						rawDataItem.Consumption.data.periods[i]
					));
				}

				accounts.Add(
					new AccountData(
						rawDataItem.AccountId,
						gasLines.OrderBy(l => l.Month).ToList(),
						deliveryLines.OrderBy(l => l.Month).ToList(),
						consumption));
			}

			return accounts;

			static List<MonthlyLine> GetLines(IReadOnlyCollection<string> lines)
			{
				var monthlyLines = new List<MonthlyLine>();

				foreach (var line in lines)
				{
					var doc = new HtmlAgilityPack.HtmlDocument();
					doc.LoadHtml(line);

					var lineBlocks = doc.DocumentNode.Descendants("div")
						.Where(el => el.HasClass("calc-history-table__row"))
						.ToList();

					foreach (var lineBlock in lineBlocks)
					{
						var children = lineBlock.ChildNodes
							.Where(el => el.Name == "div")
							.ToList();

						var month = DateTime.Parse(children[0].InnerText, _cultureInfo);
						var balancePeriodStart = children[1].InnerText.Parse();
						var paid = children[2].InnerText.Parse();
						var subs = children[3].InnerText.Parse();
						var credited = children[4].InnerText.Parse();
						var recalculated = children[5].InnerText.Parse();
						var penalty = children[6].InnerText.Parse();
						var balancePeriodEnd = children[7].InnerText.Parse();

						var monthlyLine = new MonthlyLine
							(month, balancePeriodStart, paid, subs, credited, recalculated, penalty, balancePeriodEnd);

						monthlyLines.Add(monthlyLine);
					}
				}

				return monthlyLines;
			}
		}
	}
}