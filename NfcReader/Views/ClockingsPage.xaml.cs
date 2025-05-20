using NfcReader.ViewModels;

namespace NfcReader.Views;

public partial class ClockingsPage : ContentPage
{
    public ClockingsPage(ClockingPageViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
}