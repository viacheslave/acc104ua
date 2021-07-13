using System;

namespace acc104ua
{
	internal sealed record MonthlyLine
	(
		DateTime Month,
		double BalancePeriodStart,
		double Paid,
		double Subs,
		double Credited,
		double Recalculated,
		double Penalty,
		double BalancePeriodEnd
	);
}