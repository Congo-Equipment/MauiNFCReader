using NfcReader.ViewModels;

namespace NfcReader.Views;

public partial class ActivityTrackingPage : ContentPage
{
	public ActivityTrackingPage(ActivityTrackingPageViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
    }
}