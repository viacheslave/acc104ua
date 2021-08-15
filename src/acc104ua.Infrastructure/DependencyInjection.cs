using acc104ua.Application;
using Microsoft.Extensions.DependencyInjection;

namespace acc104ua.Infrastructure
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddServices(this IServiceCollection services)
		{
			services.AddTransient<ICommandProcessor, CommandProcessor>();
			services.AddTransient<IAuthClient, AuthClient>();
			services.AddTransient<IDataProvider, DataProvider>();
			services.AddTransient<IDataExporter, DataExporter>();
			services.AddTransient<IDataTransformer, DataTransformer>();
			services.AddTransient<ILogger, Logger>();

			return services;
		}
	}
}
