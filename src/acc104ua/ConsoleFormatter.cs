using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace acc104ua
{
	internal static class ConsoleFormatter
	{
		private static readonly CultureInfo _cultureInfo = CultureInfo.GetCultureInfo("uk-UA");

		public static string GetOutput(IReadOnlyCollection<AccountDataRawDto> data)
		{
			var sb = new StringBuilder();

			foreach (var dataItem in data)
			{
				sb.AppendLine();
				sb.AppendLine(new string('-', 70));
				sb.AppendLine($"Account {dataItem.AccountId}");
				sb.AppendLine(new string('-', 70));

				var rows = new (string title, string value)[]
				{
					("БАЛАНС ГАЗ", $"{dataItem.FrontData.BalanceGas.ToString("C", _cultureInfo)}"),
					("БАЛАНС ДОСТАВКА", $"{dataItem.FrontData.BalanceDelivery.ToString("C", _cultureInfo)}"),
					($"СПОЖИВАННЯ {dataItem.FrontData.ConsumptionPeriod}", $"{dataItem.FrontData.Consumption} м³"),
					($"ГАЗ {dataItem.FrontData.GasPriceDescription}", $"{dataItem.FrontData.GasPrice.ToString("C", _cultureInfo)}"),
					($"ДОСТАВКА {dataItem.FrontData.DeliveryBillDescription}", $"{dataItem.FrontData.DeliveryBill.ToString("C", _cultureInfo)}"),
					($"ДОСТАВКА {dataItem.FrontData.DeliveryMonthlyPowerDescription}", $"{dataItem.FrontData.DeliveryMonthlyPower} м³")
				};

				foreach (var row in rows)
					sb.AppendLine($"{row.title,-48} : {row.value}");
			}

			return sb.ToString();
		}
	}
}
