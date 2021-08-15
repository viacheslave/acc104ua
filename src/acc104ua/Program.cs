using acc104ua.Application;
using acc104ua.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace acc104ua
{
	static class Program
	{
		async static Task Main(string[] args)
		{
			Console.OutputEncoding = System.Text.Encoding.UTF8;

			var services = new ServiceCollection();
			services.AddServices();

			await ProcessMode(
				services.BuildServiceProvider().GetRequiredService<ICommandProcessor>(),
				args[0], args);

			Console.WriteLine("All Done.");
		}

		private static async Task ProcessMode(ICommandProcessor commandProcessor, string option, string[] args)
		{
			switch (option)
			{
				case "--export":
					await commandProcessor.ExportData(args.Skip(1).ToArray());
					break;
				default:
					var rawData = await commandProcessor.ShowFrontPage(args);
					var output = ConsoleFormatter.GetOutput(rawData);
					Console.WriteLine(output);
					break;
			}
		}
	}
}
