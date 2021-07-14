using System;
using System.Collections.Generic;

namespace acc104ua
{
	internal class MonthlyConsumptionDataDto
	{
		public float[] volumes { get; set; }
		public float[] power { get; set; }
		public string[] periods { get; set; }
	}
}