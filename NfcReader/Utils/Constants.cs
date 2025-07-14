using CommunityToolkit.Maui.Core;

namespace NfcReader.Utils
{
    public class Constants
    {
        public static string BD_PATH = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static string DB_NAME = "nfcReader.db";
        public static string DB_PATH = Path.Combine(BD_PATH, DB_NAME);
        public static string SYS_PASSWORD = "sys.Adm1n.";
        public static SnackbarOptions SnackbarSucceesStyle = new SnackbarOptions
        {
            Font = Microsoft.Maui.Font.Default,
            BackgroundColor = Color.FromRgba(254,185,16,1),
            TextColor = Colors.White
        };
       
        public static SnackbarOptions SnackbarFailedStyle = new SnackbarOptions
        {
            Font = Microsoft.Maui.Font.Default,
            BackgroundColor = Color.FromRgba(220,53,69,1),
            TextColor = Colors.White
        };
        
        public static SnackbarOptions SnackbarDefaultStyle = new SnackbarOptions
        {
            Font = Microsoft.Maui.Font.Default,
            BackgroundColor = Color.FromRgba(120,120,120,1),
            TextColor = Colors.Black
        };
#if DEBUG
        public static string BASE_API = "http://10.0.15.57:1010/api";//dev
        //public static string BASE_API = "http://10.0.15.177:1010/api";//prod
#elif RELEASE
        public static string BASE_API = "http://10.0.15.177:1010/api";//prod
#else
        public static string BASE_API = "https://10.15.213.165:45456/api";//local dev
#endif
    }
}
