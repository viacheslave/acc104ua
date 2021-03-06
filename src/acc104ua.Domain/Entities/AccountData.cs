using System;
using System.Collections.Generic;

namespace acc104ua.Domain
{
	public sealed record AccountData
	(
		string AccountId,
		IReadOnlyCollection<MonthlyLine> GasLines,
		IReadOnlyCollection<MonthlyLine> DeliveryLines,
		IReadOnlyCollection<MonthlyConsumption> Consumption
	);
}