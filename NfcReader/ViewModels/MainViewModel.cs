using AsyncAwaitBestPractices;
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

            if (string.IsNullOrWhiteSpace(InputStaffId) && !IsClockingMode)
            {
                AppShell.Current.DisplayAlert("NFC", "Please fill first the staffId(Matricule)", "OK");
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
                StaffId = InputStaffId,
                ClockingTime = DateTime.Now,
            });

            if (!result.Success)
            {
                await AppShell.Current.DisplayAlert("NFC", result.Message, "OK");
            }
            else
            {
                await AppShell.Current.DisplayAlert("NFC", result.Message, "OK");
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
                    await AppShell.Current.DisplayAlert("NFC", "Clocking mode is enabled please desable it first before synchronization", "OK");
                    return;
                }
                var result = await _registrationService.SaveAndSync();
                await AppShell.Current.DisplayAlert("NFC", result.Message, "OK");
            }
            catch (Exception ex)
            {
                await AppShell.Current.DisplayAlert("NFC", "Synchronization failed", "OK");
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
                await AppShell.Current.DisplayAlert("NFC", "Error while refreshing data...", "OK");
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
                await AppShell.Current.DisplayAlert("NFC", "Error while refreshing data...", "OK");
            }
        }


        [RelayCommand]
        private async Task SaveAndSync()
        {
            if (string.IsNullOrWhiteSpace(NfcBadgeTagInfo) || string.IsNullOrWhiteSpace(InputStaffId))
            {
                await AppShell.Current.DisplayAlert("NFC", "Please fill first the staffId(Matricule)", "OK");
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

                await AppShell.Current.DisplayAlert("NFC", "Saved successfully", "OK");

                Recordings = [.. await _registrationService.GetLocalRecordings()];
            }
            else
            {
                await AppShell.Current.DisplayAlert("NFC", "Error saving the recording", "OK");
            }

        }
    }
}
