namespace NfcReader.Shared
{
    public class Response<Tdata> where Tdata : class
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public Tdata? Data { get; set; }
    }
}
