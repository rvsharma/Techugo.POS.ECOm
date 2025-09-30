using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.IO;
using System.Windows;
using Techugo.POS.ECom.Model;

namespace Techugo.POS.ECOm
{
    public partial class App : Application
    {
        public static IServiceProvider? ServiceProvider { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration configuration = builder.Build();

            var services = new ServiceCollection();
            services.Configure<ApiSettings>(configuration.GetSection("ApiSettings"));

            ServiceProvider = services.BuildServiceProvider();
        }
    }
}