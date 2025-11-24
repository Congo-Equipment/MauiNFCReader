using NfcReader.ViewModels;

namespace NfcReader.Views;

public partial class MeetingActivity : ContentPage
{
	public MeetingActivity(MeetingActivityViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
	
	protected override void OnAppearing()
	{
		base.OnAppearing();
		
		// Set the current date
		DateLabel.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy");
	}
}