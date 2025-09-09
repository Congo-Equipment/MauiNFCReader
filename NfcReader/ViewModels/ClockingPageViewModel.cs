using AsyncAwaitBestPractices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NfcReader.Models;
using NfcReader.Services.Interfaces;
using System.Collections.ObjectModel;

namespace NfcReader.ViewModels
{
    public partial class ClockingPageViewModel : ViewModeBase
    {
        private readonly IRegistrationService _registrationService;

        [ObservableProperty]
        private ObservableCollection<RawClocking>? _clockings;

        public ClockingPageViewModel(IRegistrationService registrationService)
        {
            _registrationService = registrationService;

            Initilize().SafeFireAndForget();
        }


        private async Task Initilize()
        {
            try
            {
                IsBusy = true;
                var data = await _registrationService.GetLocalClockings();
                Clockings = new ObservableCollection<RawClocking>(data.OrderByDescending(x => x.Created));
                IsBusy = false;
            }
            catch (Exception ex)
            {
                await AppShell.Current.DisplayAlert("NFC", ex.Message, "OK");
            }
        }

        [RelayCommand]
        private async Task Refresh()
        {
            await Initilize();
        }
    }
}
