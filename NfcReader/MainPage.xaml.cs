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


        private void OnCounterClicked(object? sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);

        }
    }
}
