using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NfcReader.Models;
using NfcReader.Services.Interfaces;
using System.Collections.ObjectModel;

namespace NfcReader.ViewModels
{
    public partial class SettingsPageViewModel : ViewModeBase
    {
        private readonly IRegistrationService _registrationService;
        private readonly ICustomApi _customApi;

        [ObservableProperty]
        private ObservableCollection<Recording>? _recordings;

        [ObservableProperty]
        private int _recordingCount;

        public SettingsPageViewModel(IRegistrationService registrationService, ICustomApi customApi)
        {
            _registrationService = registrationService;
            _customApi = customApi;

            //Initialize().SafeFireAndForget();
        }

        [RelayCommand]
        async Task Initialize()
        {
            var canInitialize = await AppShell.Current.DisplayAlert("NFC", "Are you sure you want to initialize?", "Yes", "No");
            if (!canInitialize)
                return;

            try
            {
                await Task.Delay(300);
                IsBusy = true;
                Recordings = new ObservableCollection<Recording>();
                RecordingCount = 0;
                await foreach (var item in _customApi.GetRecordings())
                {
                    if (item is not null)
                    {
                        Recordings.Add(item);
                        RecordingCount++;
                    }
                }

                var response = await _registrationService.SaveAndSync(Recordings);

                IsBusy = false;

                await AppShell.Current.DisplaySnackbar(response.Message ?? "Succeeded", duration: TimeSpan.FromSeconds(3), visualOptions: Utils.Constants.SnackbarSucceesStyle);
            }
            catch (Exception ex)
            {
                IsBusy = false;
                await AppShell.Current.DisplaySnackbar(ex.Message ?? "Failed", duration: TimeSpan.FromSeconds(3), visualOptions: Utils.Constants.SnackbarFailedStyle);
            }
        }

        [RelayCommand]
        async Task ClearData()
        {
            var prompt = await AppShell.Current.DisplayPromptAsync("NFC", "Enter the password to clear all data:", "Clear", "Cancel", "Password", -1, Keyboard.Text, "");

            if (prompt != null && !prompt.Equals(Utils.Constants.SYS_PASSWORD))
            {
                await AppShell.Current.DisplaySnackbar("Incorrect password. Please try again.", duration: TimeSpan.FromSeconds(3), visualOptions: Utils.Constants.SnackbarDefaultStyle);
                return;
            }

            var canClear = await AppShell.Current.DisplayAlert("NFC", "Are you sure you want to clear all data?", "Yes", "No");
            if (!canClear)
                return;

            try
            {
                IsBusy = true;
                var result = await _registrationService.ClearData();
                if (result.Success)
                {
                    Recordings?.Clear();
                }

                IsBusy = false;

                await AppShell.Current.DisplaySnackbar(result.Message ?? "Succeeded", duration: TimeSpan.FromSeconds(3), visualOptions: Utils.Constants.SnackbarSucceesStyle);
            }
            catch (Exception ex)
            {
                IsBusy = false;
                await AppShell.Current.DisplaySnackbar(ex.Message ?? "Operation Failed", duration: TimeSpan.FromSeconds(3), visualOptions: Utils.Constants.SnackbarFailedStyle);
            }
        }


    }
}
