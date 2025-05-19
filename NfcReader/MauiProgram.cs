using Material.Components.Maui.Extensions;
using Microsoft.Extensions.Logging;
using NfcReader.Services;
using NfcReader.Services.Interfaces;
using NfcReader.ViewModels;

namespace NfcReader
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMaterialComponents()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddScoped<MainPage>();
            builder.Services.AddTransient<MainViewModel>();

            /* service registration */
            builder.Services.AddSingleton<IRegistrationService, RegistrationService>();

            return builder.Build();
        }
    }
}
