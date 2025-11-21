using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Techugo.POS.ECom.Model;

namespace Techugo.POS.ECOm
{
    public partial class App : Application
    {
        public static IServiceProvider? ServiceProvider { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Set primary color to black at runtime
            var paletteHelper = new PaletteHelper();
            var theme = paletteHelper.GetTheme(); // FIX: Use 'var' instead of 'ITheme'

            // SetPrimaryColor extension accepts a Color
            theme.SetPrimaryColor(Colors.Black);

            // Optional: set secondary/accent to black as well
            theme.SetSecondaryColor((Color)ColorConverter.ConvertFromString("#FF000000"));

            paletteHelper.SetTheme(theme);

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration configuration = builder.Build();

            var services = new ServiceCollection();
            services.Configure<ApiSettings>(configuration.GetSection("ApiSettings"));

            ServiceProvider = services.BuildServiceProvider();

            var appFont = new FontFamily("Segoe UI, 'Segoe UI Emoji', 'Segoe UI Symbol', 'Noto Color Emoji', 'Apple Color Emoji'");

            // Make Control/TextBlock/TextElement default to the specified font family
            // C#
            var style = new Style(typeof(Control));
            style.Setters.Add(new Setter(Control.FontFamilyProperty, appFont));
            Application.Current.Resources[typeof(Control)] = style;

        }
    }
}