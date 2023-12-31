using System.Text;

using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;

namespace IntegorTelegramBotListeningService
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

			IHostBuilder builder = Host
				.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});

			IHost host = builder.Build();
			host.Run();
		}
	}
}