using System;
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

		public async Task<IReadOnlyCollection<AccountDataRawDto>> GrabRaw(Utility utility, Dates dates)
		{
			// parse raw HTML
			var accounts = await GetAccounts();

			var data = new List<AccountDataRawDto>();

			foreach (var account in accounts)
			{
				var accountDataRawDto = await GetAccountDataRaw(utility, account, dates);
				data.Add(accountDataRawDto);
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

		private async Task<AccountDataRawDto> GetAccountDataRaw(Utility utility, AccountInfo account, Dates dates)
		{
			// needs extra call
			var pages = await GetNumberOfPages(utility, account, dates);
			var htmls = new List<string>();

			for (var page = 1; page <= pages; ++page)
			{
				var content = await GetCalculationsData(utility, account, dates, page);
				htmls.Add(content);
			}

			return new AccountDataRawDto(account.Id, htmls);
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
	}
}
