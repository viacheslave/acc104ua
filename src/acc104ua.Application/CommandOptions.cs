using System;

namespace acc104ua.Application
{
	internal record Options
	(
		string Login,
		string Password,
		DateTime StartDate,
		DateTime EndDate
	);
}
