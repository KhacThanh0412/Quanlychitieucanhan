using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using Microsoft.Maui.Controls.Compatibility.Hosting;
using UraniumUI;

namespace Quanlychitieu
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<App>().RegisterCompatibilityRenderer().ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddMaterialIconFonts();
                fonts.AddFontAwesomeIconFonts();
            })
                .ConfigureEssentials(essentials =>
                {
                    essentials
                    .AddAppAction("add_flow_out", "Add Flow Out", "Add a Flow Out")
                    //.AddAppAction("add_flow_in", "Add Flow In", "Add a Flow In", "request_money_d.png")
                    .OnAppAction(App.HandleAppActions);
                })

            .UseUraniumUI()
            .UseUraniumUIMaterial()
            .UseUraniumUIBlurs()
            .UseMauiCommunityToolkit();

            builder.Services.AddApplication();
#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}