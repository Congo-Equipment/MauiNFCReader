namespace NfcReader.Utils
{
    public class Constants
    {
        public static string BD_PATH = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static string DB_NAME = "nfcReader.db";
        public static string DB_PATH = Path.Combine(BD_PATH, DB_NAME);
        public static string BASE_API = "https://10.15.213.165:45455/api";

    }
}
