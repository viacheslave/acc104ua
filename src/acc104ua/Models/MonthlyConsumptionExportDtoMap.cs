using System;
using CsvHelper.Configuration;

namespace acc104ua
{
	/// <summary>
	///		CSV class map
	/// </summary>
	internal sealed class MonthlyConsumptionExportDtoMap : ClassMap<MonthlyConsumptionExportDto>
	{
		public MonthlyConsumptionExportDtoMap()
		{
			Map(m => m.Period).Index(0)
				.Name("Період")
				.Convert(o => o.Value.Period.ToString("yyyy-MM-dd"));

			Map(m => m.Power).Index(1)
				.Name("Потужність");

			Map(m => m.Volume).Index(2)
				.Name("Об'єм");
		}
	}
}