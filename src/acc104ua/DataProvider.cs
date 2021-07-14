﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;

namespace acc104ua
{
	internal sealed class DataProvider
	{
		private readonly AuthCookie _authCookies;

		public DataProvider(AuthCookie authCookie)
		{
			_authCookies = authCookie ?? throw new ArgumentNullException(nameof(authCookie));
		}

		public async Task<IReadOnlyCollection<AccountDataRawDto>> GrabRaw(Dates dates)
		{
			// parse raw HTML
			var accounts = await GetAccounts();

			var data = new List<AccountDataRawDto>();

			foreach (var account in accounts)
			{
				var gasLines = await GetUtilityLines(Utility.Gas, account, dates);
				var deliveryLines = await GetUtilityLines(Utility.Delivery, account, dates);
				var consumption = await GetConsumption(account);
				var frontData = await GetFrontData(account);

				data.Add(
					new AccountDataRawDto(account.Id, gasLines, deliveryLines, consumption, frontData));
			}

			return data;
		}

		private async Task<IReadOnlyCollection<AccountInfo>> GetAccounts()
		{
			var content = await new Url("https://ok.104.ua/ua/")
				.WithCookie(AuthCookie.Key, _authCookies.Value)
				.GetStringAsync();

			var doc = new HtmlAgilityPack.HtmlDocument();
			doc.LoadHtml(content);

			var blockAccounts = doc.GetElementbyId("accounts__lists");
			var hrefAccounts = blockAccounts.ChildNodes.Where(a => a.Name == "a").ToList();

			var accounts = new List<AccountInfo>();

			foreach (var href in hrefAccounts)
			{
				var accountId = href.Descendants("span")
					.Where(s => s.HasClass("num"))
					.Select(s => s.InnerText)
					.First();

				var address = href.Descendants("div")
					.Where(s => s.HasClass("account-popup-item__address"))
					.Select(s => s.InnerText)
					.First();

				accounts.Add(new AccountInfo(accountId, address));
			}

			return accounts;
		}

		private async Task<IReadOnlyCollection<string>> GetUtilityLines(Utility utility, AccountInfo account, Dates dates)
		{
			// needs extra call
			var pages = await GetNumberOfPages(utility, account, dates);
			var htmls = new List<string>();

			for (var page = 1; page <= pages; ++page)
			{
				var content = await GetCalculationsData(utility, account, dates, page);
				htmls.Add(content);
			}

			return htmls;
		}

		private async Task<int> GetNumberOfPages(Utility utility, AccountInfo account, Dates dates)
		{
			var content = await GetCalculationsData(utility, account, dates);

			var doc = new HtmlAgilityPack.HtmlDocument();
			doc.LoadHtml(content);

			return doc.DocumentNode.Descendants("a")
				.Count(el => el.HasClass("history-full-pagination__num"));
		}

		private async Task<string> GetCalculationsData(Utility utility, AccountInfo account, Dates dates, int page = 1)
		{
			var apiPart = utility switch
			{
				Utility.Delivery => "delivery",
				Utility.Gas => "gas",
				_ => throw new NotImplementedException()
			};

			await new Url("https://ok.104.ua/ua/ajx/individual/account/active/set")
				 .WithCookie(AuthCookie.Key, _authCookies.Value)
				 .PostUrlEncodedAsync(new
				 {
					 account_no = account.Id
				 });

			var content = await new Url($"https://ok.104.ua/ua/ajx/individual/calculations/history/table/{apiPart}/calculations")
				.WithCookie(AuthCookie.Key, _authCookies.Value)
				.PostUrlEncodedAsync(new Dictionary<string, object>
				{
					["page"] = page,
					["epp"] = 100,
					["filters[start_date]"] = dates.StartDate.ToString("MM.yyyy"),
					["filters[end_date]"] = dates.EndDate.ToString("MM.yyyy")
				})
				.ReceiveJson();

			return content.data.html;
		}

		private async Task<MonthlyConsumptionDto> GetConsumption(AccountInfo account)
		{
			var data = await new Url("https://ok.104.ua/ua/ajx/individual/main/gasConsumptionMonthly")
				.WithCookie(AuthCookie.Key, _authCookies.Value)
				.PostUrlEncodedAsync(new
				{
					account_no = account.Id
				})
				.ReceiveJson<MonthlyConsumptionDto>();

			return data;
		}

		private async Task<FrontPageDto> GetFrontData(AccountInfo account)
		{
			var content = await new Url("https://ok.104.ua/ua/")
				.WithCookie(AuthCookie.Key, _authCookies.Value)
				.GetStringAsync();

			var doc = new HtmlAgilityPack.HtmlDocument();
			doc.LoadHtml(content);

			var el = doc.DocumentNode.Descendants("div")
				.Where(el => el.HasClass("readings-widget__info-price"))
				.ToList();

			var balanceGas = el[0].InnerText.Parse();
			var balanceDelivery = el[1].InnerText.Parse();

			el = doc.DocumentNode.Descendants("div")
				.Where(el => el.HasClass("chart-widget__volume"))
				.ToList();

			var consumption = el[0].InnerText.Parse();

			el = doc.DocumentNode.Descendants("div")
				.Where(el => el.HasClass("chart-widget__period"))
				.ToList();

			var consumptionPeriod = el[0].InnerText.Strip();

			var elnums = doc.DocumentNode.Descendants("div")
				.Where(el => el.HasClass("service-widget__info-num"))
				.ToList();

			var eltitles = doc.DocumentNode.Descendants("div")
				.Where(el => el.HasClass("service-widget__info-title"))
				.ToList();

			var gasPrice = elnums[0].InnerText.Parse();
			var gasPriceDescription = eltitles[0].InnerText.Strip();

			var deliveryBill = elnums[1].InnerText.Parse();
			var deliveryBillDescription = eltitles[1].InnerText.Strip();

			var deliveryMonthlyPower = elnums[2].InnerText.Parse();
			var deliveryMonthlyPowerDescription = eltitles[2].InnerText.Strip();

			return new FrontPageDto
			(
				BalanceGas: balanceGas,
				BalanceDelivery: balanceDelivery,
				Consumption: consumption,
				ConsumptionPeriod: consumptionPeriod,
				GasPrice: gasPrice,
				GasPriceDescription: gasPriceDescription,
				DeliveryBill: deliveryBill,
				DeliveryBillDescription: deliveryBillDescription,
				DeliveryMonthlyPower: deliveryMonthlyPower,
				DeliveryMonthlyPowerDescription: deliveryMonthlyPowerDescription 
			);
		}
	}
}
