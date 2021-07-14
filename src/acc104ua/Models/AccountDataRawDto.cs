using System;
using System.Collections.Generic;

namespace acc104ua
{
	internal sealed record AccountDataRawDto
	(
		string AccountId,
		IReadOnlyCollection<string> GasLines,
		IReadOnlyCollection<string> DeliveryLines,
		MonthlyConsumptionDto Consumption
	);
}