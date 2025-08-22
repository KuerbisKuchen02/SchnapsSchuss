using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;

namespace SchnapsSchuss;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit(options =>
            {
                options.SetPopupDefaults(new DefaultPopupSettings
                {
                    CanBeDismissedByTappingOutsideOfPopup = true,
                    BackgroundColor = Color.FromArgb("1f1f1f"), 
                });
            })
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}