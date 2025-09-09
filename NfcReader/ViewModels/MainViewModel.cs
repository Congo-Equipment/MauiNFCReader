using Android.Media;
using AsyncAwaitBestPractices;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NfcReader.Models;
using NfcReader.Services.Interfaces;
using Plugin.NFC;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace NfcReader.ViewModels
{
    public partial class MainViewModel : ViewModeBase
    {
        private readonly IRegistrationService _registrationService;


        [ObservableProperty]
        private ObservableCollection<Recording>? _recordings;

        [ObservableProperty]
        private string? _nfcBadgeTagInfo;

        [ObservableProperty]
        private bool _showSaveButton = false;

        [ObservableProperty]
        private string _inputStaffId;

        [ObservableProperty]
        private bool _isClockingMode = false;

        [ObservableProperty]
        private string _message = string.Empty;

        [ObservableProperty]
        private Ringtone? _ringSound;



        public MainViewModel(IRegistrationService registrationService)
        {
            _registrationService = registrationService;

            //Initialize NFC
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

                    Recordings = [.. await _registrationService.GetLocalRecordings()];

                    var instance = Platform.CurrentActivity;
                    Android.Net.Uri uri = RingtoneManager.GetDefaultUri(RingtoneType.Notification);
                    RingSound = RingtoneManager.GetRingtone(instance.ApplicationContext, uri);
                }
                else
                {
                    CrossNFC.Current.OnMessageReceived -= Current_OnMessageReceived;
                    await AppShell.Current.DisplaySnackbar("NFC is not enabled\n\r Please enable NFC in your device settings", duration: TimeSpan.FromSeconds(3), visualOptions: Utils.Constants.SnackbarFailedStyle);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing NFC: {ex.Message}");
                await AppShell.Current.DisplaySnackbar("Error initializing NFC", duration: TimeSpan.FromSeconds(3), visualOptions: Utils.Constants.SnackbarFailedStyle);
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
                Debug.WriteLine($"Error starting NFC listening: {ex.Message}");
                await AppShell.Current.DisplaySnackbar("Error starting NFC listening", duration: TimeSpan.FromSeconds(3), visualOptions: Utils.Constants.SnackbarFailedStyle);
            }
        }

        void Current_OnMessageReceived(ITagInfo tagInfo)
        {
            if (tagInfo == null)
            {
                AppShell.Current.DisplaySnackbar("No tag found", duration: TimeSpan.FromSeconds(3), visualOptions: Utils.Constants.SnackbarFailedStyle);
                return;
            }

            if (string.IsNullOrWhiteSpace(InputStaffId) && !IsClockingMode)
            {
                AppShell.Current.DisplaySnackbar("Please fill first the staffId(Matricule)", duration: TimeSpan.FromSeconds(3), visualOptions: Utils.Constants.SnackbarFailedStyle);
                return;
            }

            // Customized serial number
            var identifier = tagInfo.Identifier;
            var serialNumber = NFCUtils.ByteArrayToHexString(identifier, string.Empty);
            var title = !string.IsNullOrWhiteSpace(serialNumber) ? $"Tag [{serialNumber}]" : "Tag Info";

            if (!tagInfo.IsSupported)
            {
                NfcBadgeTagInfo = serialNumber;
                if (IsClockingMode)
                    Clock(serialNumber).SafeFireAndForget();
                else
                    ShowSaveButton = !string.IsNullOrWhiteSpace(NfcBadgeTagInfo) && !string.IsNullOrWhiteSpace(InputStaffId);

            }
            else if (tagInfo.IsEmpty)
            {
                AppShell.Current.DisplaySnackbar("Empty tag", duration: TimeSpan.FromSeconds(3), visualOptions: Utils.Constants.SnackbarFailedStyle);
            }
            else
            {
                var first = tagInfo.Records[0];
                AppShell.Current.DisplaySnackbar($"Tag Info: {first.ToString()}", duration: TimeSpan.FromSeconds(3), visualOptions: Utils.Constants.SnackbarDefaultStyle);
            }
        }

        private async Task Clock(string badgeId)
        {
            //IsBusy = true;
            if (string.IsNullOrWhiteSpace(badgeId))
            {
                await AppShell.Current.DisplaySnackbar("Empty tag", duration: TimeSpan.FromSeconds(3), visualOptions: Utils.Constants.SnackbarFailedStyle);
            }

            var result = await _registrationService.SaveAndSync(new RawClocking
            {
                BadgeId = badgeId,
                StaffId = InputStaffId,
                ClockingTime = DateTime.Now,
            });

            if (!result.Success)
            {
                await AppShell.Current.DisplaySnackbar(result.Message ?? "Operation completed successfully", duration: TimeSpan.FromSeconds(3), visualOptions: Utils.Constants.SnackbarFailedStyle);
            }
            else
            {
                await AppShell.Current.DisplaySnackbar(result.Message ?? "Operation completed with errors", duration: TimeSpan.FromSeconds(3), visualOptions: Utils.Constants.SnackbarDefaultStyle);
                RingSound?.Play();

            }
            //await Task.Delay(500);
            //IsBusy = false;
            //Message = result.Message;
        }

        [RelayCommand]
        private async Task GotToLogs()
        {
            await AppShell.Current.GoToAsync("ClockingsPage", true);
        }

        [RelayCommand]
        private async Task SyncExisting()
        {
            try
            {
                if (IsClockingMode)
                {
                    await AppShell.Current.DisplaySnackbar("Clocking mode is enabled, please disable it first before synchronization", duration: TimeSpan.FromSeconds(3), visualOptions: Utils.Constants.SnackbarFailedStyle);
                    return;
                }
                var result = await _registrationService.SaveAndSync();
                await AppShell.Current.DisplaySnackbar(result.Message ?? "Operation completed successfully", duration: TimeSpan.FromSeconds(3), visualOptions: Utils.Constants.SnackbarSucceesStyle);
            }
            catch (Exception ex)
            {
                await AppShell.Current.DisplaySnackbar("Synchronization failed", duration: TimeSpan.FromSeconds(3), visualOptions: Utils.Constants.SnackbarFailedStyle);
            }
        }

        [RelayCommand]
        private async Task Refresh()
        {
            try
            {
                IsBusy = true;
                Recordings = [.. await _registrationService.GetLocalRecordings()];
                IsBusy = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error while refreshing data: {ex.Message}");
                IsBusy = false;
                await AppShell.Current.DisplaySnackbar("Error while refreshing data", duration: TimeSpan.FromSeconds(3), visualOptions: Utils.Constants.SnackbarFailedStyle);
            }
        }

        [RelayCommand]
        private async Task ClearAll()
        {
            try
            {
                var dialog = await AppShell.Current.DisplayAlert("NFC", "Would you like to  clear all data, this operation is irreversible", "YES", "NO");
                if (dialog)
                {
                    IsBusy = true;
                    await _registrationService.ClearData();
                    Recordings = [.. await _registrationService.GetLocalRecordings()];
                    IsBusy = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error while clearing data: {ex.Message}");
                IsBusy = false;
                await AppShell.Current.DisplaySnackbar("Error while clearing data", duration: TimeSpan.FromSeconds(3), visualOptions: Utils.Constants.SnackbarFailedStyle);
            }
        }


        [RelayCommand]
        private async Task SaveAndSync()
        {
            if (string.IsNullOrWhiteSpace(NfcBadgeTagInfo) || string.IsNullOrWhiteSpace(InputStaffId))
            {
                await AppShell.Current.DisplaySnackbar("Please fill first the staffId(Matricule)", duration: TimeSpan.FromSeconds(3), visualOptions: Utils.Constants.SnackbarFailedStyle);
                return;
            }

            var recording = new Recording
            {
                BadgeId = NfcBadgeTagInfo,
                StaffId = InputStaffId,
                Created = DateTime.UtcNow
            };

            // Check if the cardId has been recorded for the given staffId
            if (await _registrationService.SaveAndSync(recording))
            {
                // Reset the fields after saving
                NfcBadgeTagInfo = string.Empty;
                InputStaffId = string.Empty;

                ShowSaveButton = false;
                await AppShell.Current.DisplaySnackbar("Saved successfully", duration: TimeSpan.FromSeconds(3), visualOptions: Utils.Constants.SnackbarSucceesStyle);

                // Refresh the recordings list
                Recordings = [.. await _registrationService.GetLocalRecordings()];
            }
            else
            {
                await AppShell.Current.DisplaySnackbar("Error saving the recording", duration: TimeSpan.FromSeconds(3), visualOptions: Utils.Constants.SnackbarFailedStyle);
            }

        }
    }
}
