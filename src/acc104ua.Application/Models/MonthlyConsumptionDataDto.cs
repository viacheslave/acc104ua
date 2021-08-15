using System;
using System.Collections.Generic;

namespace acc104ua.Application
{
	public class MonthlyConsumptionDataDto
	{
		public float[] volumes { get; set; }
		public float[] power { get; set; }
		public string[] periods { get; set; }
	}
}