using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Plugin.NFC;

namespace NfcReader
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            CrossNFC.Init(this);
            base.OnCreate(savedInstanceState);
        }
        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            // Handle NFC intent here if needed
            CrossNFC.OnNewIntent(intent);
        }

        protected override void OnResume()
        {
            base.OnResume();
            // Handle NFC intent here if needed
            CrossNFC.OnResume();
        }
    }
}
