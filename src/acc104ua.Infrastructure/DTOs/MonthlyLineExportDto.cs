using System;

namespace acc104ua.Infrastructure
{
	/// <summary>
	///		CSV line
	/// </summary>
	public class MonthlyLineExportDto
	{
		public string AccountId { get; set; }

		public DateTime Month { get; set; }

		public double BalancePeriodStart { get; set; }

		public double Paid { get; set; }

		public double Subs { get; set; }

		public double Credited { get; set; }

		public double Recalculated { get; set; }

		public double Penalty { get; set; }

		public double BalancePeriodEnd { get; set; }
	}
}
