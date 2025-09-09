using AsyncAwaitBestPractices;
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

                await AppShell.Current.DisplayAlert("NFC", response.Message, "OK");
            }
            catch (Exception ex)
            {
                await AppShell.Current.DisplayAlert("NFC", ex.Message, "OK");
            }
        }

        [RelayCommand]
        async Task ClearData()
        {
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

                await AppShell.Current.DisplayAlert("NFC", result.Message, "OK");
            }
            catch (Exception ex)
            {
                await AppShell.Current.DisplayAlert("NFC", ex.Message, "OK");
            }
        }


    }
}
