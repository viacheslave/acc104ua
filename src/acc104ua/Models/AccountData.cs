using System;
using System.Collections.Generic;

namespace acc104ua
{
	internal sealed record AccountData
	(
		string AccountId,
		IReadOnlyCollection<MonthlyLine> Lines
	);
}