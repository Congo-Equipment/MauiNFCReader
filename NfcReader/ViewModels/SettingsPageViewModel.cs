using AsyncAwaitBestPractices;
using CommunityToolkit.Mvvm.ComponentModel;
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

        public SettingsPageViewModel(IRegistrationService registrationService, ICustomApi customApi)
        {
            _registrationService = registrationService;
            _customApi = customApi;

            Initialize().SafeFireAndForget();
        }

        async Task Initialize()
        {
            Recordings = new ObservableCollection<Recording>();
            await foreach (var item in _customApi.GetRecordings())
            {
                if (item is not null)
                    Recordings.Add(item);
            }
        }


    }
}
