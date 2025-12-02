using AsyncAwaitBestPractices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NfcReader.Models;
using NfcReader.Services.Interfaces;
using NfcReader.Views;
using System.Collections.ObjectModel;

namespace NfcReader.ViewModels;

public partial class ActivityMenuPageViewModel : ViewModeBase
{
    private readonly ICustomApi _apiService;


    [ObservableProperty]
    private ObservableCollection<ClockingType> _clockingTypes = new();

    public ActivityMenuPageViewModel(ICustomApi apiService)
    {
        _apiService = apiService;

        InitializeAsync().SafeFireAndForget();
    }


    public async Task InitializeAsync()
    {
        IsBusy = true;
        try
        {
            //var clockingTypes = await _apiService.GetClockingTypesAsync();
            //ClockingTypes = new ObservableCollection<ClockingType>(clockingTypes);
            ClockingTypes = [
                    new(){
                        Id = Guid.CreateVersion7(),
                        Name = "Morning Announcement",
                        Description = "Record your clock in time."

                    },
                    new(){
                        Id = Guid.CreateVersion7(),
                        Name = "Lunch Break",
                        Description = "Record your lunch break time."
                    },
                    new(){
                        Id = Guid.CreateVersion7(),
                        Name = "Afternoon Announcement",
                        Description = "Record your clock out time."
                    }
                ];
            //TODO: Uncomment after API is ready
            //await foreach (var item in _apiService.GetClockingTypesAsync())
            //{
            //    ClockingTypes.Add(item!);
            //}
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., log error, show message to user)
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SelectOption(ClockingType selectedType)
    {
        if (selectedType is null)
            return;

        var @params = new Dictionary<string, object>()
        {
            {"type", selectedType }
        };

        // Navigate to the detail page or perform other actions based on the selected option
        await Shell.Current.GoToAsync($"{nameof(ActivityTrackingPage)}", @params);
    }
}
