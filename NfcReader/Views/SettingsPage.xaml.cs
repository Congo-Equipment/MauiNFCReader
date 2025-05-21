using NfcReader.ViewModels;

namespace NfcReader.Views;

public partial class SettingsPage : ContentPage
{
    private readonly SettingsPageViewModel _viewModel;

    public SettingsPage(SettingsPageViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        BindingContext = _viewModel;
    }
}