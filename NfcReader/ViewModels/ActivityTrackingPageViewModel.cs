using Android.Media;
using AsyncAwaitBestPractices;
using CommunityToolkit.Mvvm.ComponentModel;
using NfcReader.Models;
using NfcReader.Services.Interfaces;
using Plugin.NFC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NfcReader.ViewModels
{
    public partial class ActivityTrackingPageViewModel : ViewModeBase
    {
        private readonly IRegistrationService _registrationService;

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

        public ActivityTrackingPageViewModel(IRegistrationService registrationService)
        {
            _registrationService = registrationService;

            // Initialize NFC
            Initialize().SafeFireAndForget();
        }



        private async Task Initialize()
        {
            try
            {
                CrossNFC.Legacy = false;
                if (CrossNFC.Current.IsEnabled)
                {
                    //await AppShell.Current.DisplayAlert("NFC", "NFC is enabled", "OK");
                    await AutoStartAsync();

                    ClockingsCount = await _registrationService.GetTodayClockingCount();

                    var instance = Platform.CurrentActivity;
                    Android.Net.Uri uri = RingtoneManager.GetDefaultUri(RingtoneType.Notification);
                    RingSound = RingtoneManager.GetRingtone(instance.ApplicationContext, uri);

                    Android.Net.Uri uriFailed = RingtoneManager.GetDefaultUri(RingtoneType.Alarm);
                    RingSoundFailed = RingtoneManager.GetRingtone(instance.ApplicationContext, uriFailed);
                }
                else
                {
                    await AppShell.Current.DisplayAlert("NFC", "NFC is not enabled", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing NFC: {ex.Message}");
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
                Clock(serialNumber).SafeFireAndForget();

            }
            else if (tagInfo.IsEmpty)
            {
                AppShell.Current.DisplayAlert("NFC", "Empty tag", "OK");
            }
            else
            {
                var first = tagInfo.Records[0];
                AppShell.Current.DisplayAlert("NFC", first.ToString(), "OK");
            }
        }

        private async Task Clock(string badgeId)
        {
            //IsBusy = true;
            if (string.IsNullOrWhiteSpace(badgeId))
            {
                await AppShell.Current.DisplayAlert("NFC", "Empty tag", "OK");
            }

            var result = await _registrationService.SaveAndSync(new RawClocking
            {
                BadgeId = badgeId,
                ClockingTime = DateTime.Now,
            });

            if (!result.Success)
            {
                //await AppShell.Current.DisplayAlert("NFC", result.Message, "OK");
                IsSuccessful = true;
                ClockingsCount++;// Increment the count of clockings based on the successful clocking and the existing count from the service
                RingSound?.Play();
                await Task.Delay(1000);// Stop the sound after 1 second
                RingSoundFailed?.Stop();
            }
            else
            {
                //await AppShell.Current.DisplayAlert("NFC", result.Message, "OK");
                IsSuccessful = false;
                RingSoundFailed?.Play();
                await Task.Delay(1000);// Stop the sound after 1 second
                RingSound?.Stop();
            }
        }

    }
}
