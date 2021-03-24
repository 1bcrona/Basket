using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Basket.API
{
    public class Program
    {
        #region Public Methods

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        #endregion Public Methods
    }
}