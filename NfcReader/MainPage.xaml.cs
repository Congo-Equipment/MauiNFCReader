using AsyncAwaitBestPractices;
using NfcReader.ViewModels;
using Plugin.NFC;

namespace NfcReader
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        private readonly MainViewModel _viewModel;

        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            //BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            BindingContext = _viewModel;
        }

        private void fabMainBtn_Clicked(object sender, TouchEventArgs e)
        {
            recordingSheet.Show();
        }
    }
}
