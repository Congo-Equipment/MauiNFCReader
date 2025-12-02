using NfcReader.Views;

namespace NfcReader
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(ActivityTrackingPage), typeof(ActivityTrackingPage));
        }
    }
}
