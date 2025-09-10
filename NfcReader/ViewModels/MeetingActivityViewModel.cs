using Android.Media;
using AsyncAwaitBestPractices;
using CommunityToolkit.Mvvm.ComponentModel;
using NfcReader.Services.Interfaces;
using NfcReader.Shared;
using Plugin.NFC;

namespace NfcReader.ViewModels
{
    public partial class MeetingActivityViewModel : ViewModeBase
    {
        private readonly IClockingService _clockingService;

        [ObservableProperty]
        private int clockingsCount;

        [ObservableProperty]
        private bool? isSuccessful;

        [ObservableProperty]
        private Ringtone? _ringSound;

        [ObservableProperty]
        private Ringtone? _ringSoundFailed;

        [ObservableProperty]
        private string? _nfcBadgeTagInfo;

        [ObservableProperty]
        private string? _currentBadgeOwner;

        public MeetingActivityViewModel(IClockingService clockingService)
        {
            _clockingService = clockingService;

            Initialize().SafeFireAndForget();
        }

        private async Task Initialize()
        {
            try
            {
                CrossNFC.Legacy = false;
                if (CrossNFC.Current.IsEnabled)
                {
                    await AutoStartAsync();
                    CurrentBadgeOwner = "Waiting for tag...";
                    // Load initial clockings count if needed

                    var instance = Platform.CurrentActivity;
                    Android.Net.Uri uri = RingtoneManager.GetDefaultUri(RingtoneType.Notification);
                    RingSound = RingtoneManager.GetRingtone(instance.ApplicationContext, uri);

                    Android.Net.Uri uriFailed = RingtoneManager.GetDefaultUri(RingtoneType.Ringtone);
                    RingSoundFailed = RingtoneManager.GetRingtone(instance.ApplicationContext, uriFailed);
                }
                else
                {
                    await AppShell.Current.DisplayAlert("NFC", "NFC is not enabled", "OK");
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
            }
        }

        async Task AutoStartAsync()
        {
            // Some delay to prevent Java.Lang.IllegalStateException "Foreground dispatch can only be enabled when your activity is resumed" on Android
            await Task.Delay(500);
            await BeginListening();
        }

        async Task BeginListening()
        {
            try
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    CrossNFC.Current.OnMessageReceived += Current_OnMessageReceived;
                    CrossNFC.Current.StartListening();
                });
            }
            catch (Exception ex)
            {
                await AppShell.Current.DisplayAlert("NFC", ex.Message, "OK");
            }
        }

        void Current_OnMessageReceived(ITagInfo tagInfo)
        {
            if (tagInfo == null)
            {
                AppShell.Current.DisplayAlert("NFC", "No tag found", "OK");
                return;
            }

            // Customized serial number
            var identifier = tagInfo.Identifier;
            var serialNumber = NFCUtils.ByteArrayToHexString(identifier, string.Empty);
            var title = !string.IsNullOrWhiteSpace(serialNumber) ? $"Tag [{serialNumber}]" : "Tag Info";

            if (!tagInfo.IsSupported)
            {
                NfcBadgeTagInfo = serialNumber;
                PunchAsync(serialNumber).SafeFireAndForget();

            }
            else if (tagInfo.IsEmpty)
            {
                Shell.Current.DisplayAlert("NFC", "Empty tag", "OK");
            }
            else
            {
                var first = tagInfo.Records[0];
                Shell.Current.DisplayAlert("NFC", first.ToString(), "OK");
            }
        }

        private async Task PunchAsync(string serialNumber, CancellationToken cancellationToken = default)
        {
            try
            {
                var hasClockedIn = await _clockingService.HasClockedInAsync(serialNumber, cancellationToken);
                if (hasClockedIn)
                {
                    await Shell.Current.DisplayAlert("NFC", "You have already clocked in.", "OK");
                    return;
                }

                var response = await _clockingService.SaveClockingAsync(serialNumber, "", cancellationToken);
                if (response.Success)
                {
                    await Shell.Current.DisplayAlert("NFC", "Clocking successful.", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("NFC", response.Message, "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("NFC", ex.Message, "OK");
            }
        }
    }
}
