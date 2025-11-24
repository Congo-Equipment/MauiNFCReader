using Android.Media;
using AsyncAwaitBestPractices;
using CommunityToolkit.Mvvm.ComponentModel;
using NfcReader.Models;
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
        private Employee? currentEmployee;

        [ObservableProperty]
        private string currentEmployeeName = "Waiting for badge...";

        [ObservableProperty]
        private string statusMessage = "Ready to scan";

        [ObservableProperty]
        private bool isProcessing = false;

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
                // Load initial clockings count
                await LoadTodayClockingsCount();

                CrossNFC.Legacy = false;
                if (CrossNFC.Current.IsEnabled)
                {
                    await AutoStartAsync();
                    CurrentBadgeOwner = "Waiting for tag...";

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
                StatusMessage = "Error initializing NFC";
            }
        }

        private async Task LoadTodayClockingsCount()
        {
            try
            {
                ClockingsCount = await _clockingService.TodayClockingAsync();
            }
            catch (Exception ex)
            {
                ClockingsCount = 0;
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
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    StatusMessage = "No tag found";
                    IsSuccessful = false;
                });
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
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    StatusMessage = "Empty tag";
                    IsSuccessful = false;
                });
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
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    IsProcessing = true;
                    StatusMessage = "Processing...";
                    IsSuccessful = null;
                });

                // Get employee information
                var employeeResponse = await _clockingService.GetOrFetchEmployeeInfoAsync(serialNumber, cancellationToken);
                if (!employeeResponse.Success || employeeResponse.Data == null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        CurrentEmployeeName = "Unknown Employee";
                        StatusMessage = "Employee not found";
                        IsSuccessful = false;
                        IsProcessing = false;
                        RingSoundFailed?.Play();
                    });

                    await Task.Delay(600);

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        CurrentEmployeeName = "Unknown Employee";
                        StatusMessage = "Employee not found";
                    });

                    return;
                }

                CurrentEmployee = employeeResponse.Data;
                var fullName = $"{employeeResponse.Data.Name} {employeeResponse.Data.Surname}".Trim();

                var hasClockedIn = await _clockingService.HasClockedInAsync(serialNumber, cancellationToken);
                if (hasClockedIn)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        CurrentEmployeeName = fullName;
                        StatusMessage = "Already clocked in today";
                        IsSuccessful = false;
                        IsProcessing = false;
                        RingSoundFailed?.Play();
                    });
                    return;
                }

                var response = await _clockingService.SaveClockingAsync(serialNumber, employeeResponse.Data.StaffId ?? "", cancellationToken);
                
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    CurrentEmployeeName = fullName;
                    IsProcessing = false;
                    
                    if (response.Success)
                    {
                        StatusMessage = "Clocking successful!";
                        IsSuccessful = true;
                        ClockingsCount++;
                        RingSound?.Play();
                        
                        // Auto-reset visual confirmation after 3 seconds
                        await Task.Delay(3000);
                        StatusMessage = "Ready to scan";
                        CurrentEmployeeName = "Waiting for badge...";
                        IsSuccessful = null;
                    }
                    else
                    {
                        StatusMessage = response.Message ?? "Clocking failed";
                        IsSuccessful = false;
                        RingSoundFailed?.Play();
                    }
                });
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    StatusMessage = "Error processing clocking";
                    IsSuccessful = false;
                    IsProcessing = false;
                    RingSoundFailed?.Play();
                });
            }
        }
    }
}
