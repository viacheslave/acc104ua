namespace acc104ua.Application
{
	public record FrontPageDto
	(
		double BalanceGas,
		double BalanceDelivery,
		double Consumption,
		string ConsumptionPeriod,
		double GasPrice,
		string GasPriceDescription,
		double DeliveryBill,
		string DeliveryBillDescription,
		double DeliveryMonthlyPower,
		string DeliveryMonthlyPowerDescription
	);
}