using acc104ua.Application;
using System;

namespace acc104ua.Infrastructure
{
	internal class Logger : ILogger
	{
		public void Out(string msg) => Console.WriteLine(msg);
	}
}
