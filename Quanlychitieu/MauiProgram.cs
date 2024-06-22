using CommunityToolkit.Maui;
using UraniumUI;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace Quanlychitieu
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseSkiaSharp(true)
                .UseMauiApp<App>()
                .RegisterCompatibilityRenderer()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddMaterialIconFonts();
                    fonts.AddFontAwesomeIconFonts();
                })
                .ConfigureEssentials(essentials =>
                {
                    essentials
                    .AddAppAction("add_flow_out", "Add Flow Out", "Add a Flow Out");
                })

                .UseUraniumUI()
                .UseUraniumUIMaterial()
                .UseUraniumUIBlurs()
                .UseMauiCommunityToolkit();

            builder.Services.AddApplication();
            builder.Services.AddSingleton<IPlannedExpendituresRepository, PlannedExpendituresRepository>();

            return builder.Build();
        }
    }
}