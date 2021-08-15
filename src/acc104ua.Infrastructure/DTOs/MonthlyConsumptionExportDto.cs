using System;

namespace acc104ua.Infrastructure
{
	/// <summary>
	///		CSV export model
	/// </summary>
	internal class MonthlyConsumptionExportDto
	{
		public float Volume { get; set; }

		public float Power { get; set; }

		public DateTime Period { get; set; }
	}
}