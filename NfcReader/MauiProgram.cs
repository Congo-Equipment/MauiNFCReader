using CommunityToolkit.Maui;
using Material.Components.Maui.Extensions;
using Microsoft.EntityFrameworkCore;
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
                .UseMauiCommunityToolkit()
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

            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<SettingsPageViewModel>();

            builder.Services.AddTransient<ActivityTrackingPage>();
            builder.Services.AddTransient<ActivityTrackingPageViewModel>();

            builder.Services.AddTransient<MeetingActivity>();
            builder.Services.AddTransient<MeetingActivityViewModel>();

            builder.Services.AddTransient<ActivityMenuPage>();
            builder.Services.AddTransient<ActivityMenuPageViewModel>();

            /* service registration */
            builder.Services.AddTransient<IRegistrationService, RegistrationService>();
            builder.Services.AddTransient<ICustomApi, CustomApi>();

            /* api service*/
            builder.Services.AddRefitClient<IApiService>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(Constants.BASE_API))
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                });

            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Path.Combine(Environment.GetFolderPath(folder), "nfc_reader.db");
            builder.Services.AddDbContextFactory<Contexts.ApplicationDbContext>(options =>
            {
                options.UseSqlite($"Filename={path}");
            });

            builder.Services.AddTransient<IClockingService, ClockingService>();

            return builder.Build();
        }
    }
}
