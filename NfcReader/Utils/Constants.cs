namespace NfcReader.Utils
{
    public class Constants
    {
        public static string BD_PATH = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static string DB_NAME = "nfcReader.db";
        public static string DB_PATH = Path.Combine(BD_PATH, DB_NAME);
#if DEBUG
        // public static string BASE_API = "http://10.0.15.57:1010/api";//dev
        public static string BASE_API = "http://10.0.15.177:1010/api";//prod
#elif RELEASE
        public static string BASE_API = "http://10.0.15.177:1010/api";//prod
#else
        public static string BASE_API = "https://10.15.213.165:45456/api";//local dev
#endif
    }
}
