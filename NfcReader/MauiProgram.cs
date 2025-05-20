using Material.Components.Maui.Extensions;
using Microsoft.Extensions.Logging;
using NfcReader.Services;
using NfcReader.Services.Interfaces;
using NfcReader.Utils;
using NfcReader.ViewModels;
using NfcReader.Views;
using Refit;
using Syncfusion.Maui.Toolkit.Hosting;

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
                .ConfigureSyncfusionToolkit()
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

            builder.Services.AddTransient<ClockingsPage>();
            builder.Services.AddTransient<ClockingPageViewModel>();

            /* service registration */
            builder.Services.AddTransient<IRegistrationService, RegistrationService>();

            /* api service*/
            builder.Services.AddRefitClient<IApiService>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(Constants.BASE_API))
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                });

            return builder.Build();
        }
    }
}
