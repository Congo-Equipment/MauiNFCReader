using AsyncAwaitBestPractices;
using Plugin.NFC;

namespace NfcReader
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();

        }

        protected override void OnAppearing()
        {
            CrossNFC.Legacy = false;
            if (CrossNFC.Current.IsEnabled)
            {
                DisplayAlert("NFC", "NFC is enabled", "OK");
                AutoStartAsync().SafeFireAndForget();
            }
            else
            {
                DisplayAlert("NFC", "NFC is not enabled", "OK");
            }
        }

        async Task AutoStartAsync()
        {
            // Some delay to prevent Java.Lang.IllegalStateException "Foreground dispatch can only be enabled when your activity is resumed" on Android
            await Task.Delay(500);
            await BeginListening();
        }

        async Task BeginListening()
        {
            try
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    CrossNFC.Current.OnMessageReceived += Current_OnMessageReceived;
                    CrossNFC.Current.StartListening();
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert("NFC",ex.Message,"OK");
            }
        }

        async void Current_OnMessageReceived(ITagInfo tagInfo)
        {
            if (tagInfo == null)
            {
                await DisplayAlert("NFC", "No tag found", "OK");
                return;
            }

            // Customized serial number
            var identifier = tagInfo.Identifier;
            var serialNumber = NFCUtils.ByteArrayToHexString(identifier, string.Empty);
            var title = !string.IsNullOrWhiteSpace(serialNumber) ? $"Tag [{serialNumber}]" : "Tag Info";

            if (!tagInfo.IsSupported)
            {
                await DisplayAlert("NFC", title, "OK");
            }
            else if (tagInfo.IsEmpty)
            {
                await DisplayAlert("NFC", "Empty tag", "OK");
            }
            else
            {
                var first = tagInfo.Records[0];
                await DisplayAlert("NFC", first.ToString(), "OK");
            }
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
