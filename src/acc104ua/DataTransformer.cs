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
				var monthlyLines = new List<MonthlyLine>();

				foreach (var line in rawDataItem.Lines)
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
						var balancePeriodStart = children[1].InnerText.ParseMoney();
						var paid = children[2].InnerText.ParseMoney();
						var subs = children[3].InnerText.ParseMoney();
						var credited = children[4].InnerText.ParseMoney();
						var recalculated = children[5].InnerText.ParseMoney();
						var penalty = children[6].InnerText.ParseMoney();
						var balancePeriodEnd = children[7].InnerText.ParseMoney();

						var monthlyLine = new MonthlyLine
							(month, balancePeriodStart, paid, subs, credited, recalculated, penalty, balancePeriodEnd);

						monthlyLines.Add(monthlyLine);
					}
				}

				accounts.Add(
					new AccountData(
						rawDataItem.AccountId, 
						monthlyLines.OrderBy(l => l.Month).ToList()));
			}

			return accounts;
		}
	}
}