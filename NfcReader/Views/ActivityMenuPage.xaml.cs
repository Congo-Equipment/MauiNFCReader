using NfcReader.ViewModels;

namespace NfcReader.Views;

public partial class ActivityMenuPage : ContentPage
{
    public ActivityMenuPage(ActivityMenuPageViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }
}