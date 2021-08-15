using System;
using System.Collections.Generic;

namespace acc104ua.Application
{
	public sealed record AccountDataRawDto
	(
		string AccountId,
		IReadOnlyCollection<string> GasLines,
		IReadOnlyCollection<string> DeliveryLines,
		MonthlyConsumptionDto Consumption,
		FrontPageDto FrontData
	);
}