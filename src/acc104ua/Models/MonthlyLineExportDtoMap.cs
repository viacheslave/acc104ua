using CsvHelper.Configuration;

namespace acc104ua
{
	/// <summary>
	///		CSV class map
	/// </summary>
	public sealed class MonthlyLineExportDtoMap : ClassMap<MonthlyLineExportDto>
	{
		public MonthlyLineExportDtoMap()
		{
			Map(m => m.AccountId).Index(0)
				.Name("ОС.РАХ");

			Map(m => m.Month).Index(1)
				.Name("Період")
				.Convert(o => o.Value.Month.ToString("yyyy-MM-dd"));

			Map(m => m.BalancePeriodStart).Index(2)
				.Name("Баланс (початок періоду)");

			Map(m => m.Paid).Index(3)
				.Name("Платежі");

			Map(m => m.Subs).Index(4)
				.Name("Пільги, субсидія");

			Map(m => m.Credited).Index(5)
				.Name("Нараховано");

			Map(m => m.Recalculated).Index(6)
				.Name("Перерахунки");

			Map(m => m.Penalty).Index(7)
				.Name("Пеня");

			Map(m => m.BalancePeriodEnd).Index(8)
				.Name("Баланс (кінець періоду)");
		}
	}
}
