using NfcReader.Models;
using NfcReader.Services.Interfaces;
using NfcReader.Utils;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace NfcReader.Services
{
    internal class CustomApi : ICustomApi
    {
        public async IAsyncEnumerable<Recording?> GetRecordings()
        {
            //handler = new HttpClientHandler
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            using var client = new HttpClient(handler);

            client.BaseAddress = new Uri($"{Constants.BASE_API}/clocking/recordings");
            await foreach (var item in client.GetFromJsonAsAsyncEnumerable<Recording>(client.BaseAddress))
            {
                yield return item;
            }
        }
    }
}
