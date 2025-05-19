﻿using AsyncAwaitBestPractices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NfcReader.Models;
using NfcReader.Services.Interfaces;
using Plugin.NFC;
using System.Diagnostics;

namespace NfcReader.ViewModels
{
    public partial class MainViewModel : ViewModeBase
    {
        private readonly IRegistrationService _registrationService;


        [ObservableProperty]
        private IReadOnlyCollection<Recording>? _recordings;

        [ObservableProperty]
        private string? _nfcBadgeTagInfo;

        [ObservableProperty]
        private bool _showSaveButton = false;

        [ObservableProperty]
        private string _inputStaffId;

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

            if (string.IsNullOrWhiteSpace(InputStaffId))
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
                ShowSaveButton = !string.IsNullOrWhiteSpace(NfcBadgeTagInfo) && !string.IsNullOrWhiteSpace(InputStaffId);
                //AppShell.Current.DisplayAlert("NFC", title, "OK");
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
            await _registrationService.SaveAndSync(recording);
            await AppShell.Current.DisplayAlert("NFC", "Saved successfully", "OK");
            // Reset the fields after saving
            NfcBadgeTagInfo = string.Empty;
            InputStaffId = string.Empty;
        }
    }
}
