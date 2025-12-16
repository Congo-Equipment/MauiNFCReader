using NfcReader.Models;
using NfcReader.Services.Interfaces;
using NfcReader.Shared;
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

        public async IAsyncEnumerable<ClockingType?> GetClockingTypesAsync()
        {
            //handler = new HttpClientHandler
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            using var client = new HttpClient(handler);
            client.BaseAddress = new Uri($"{Constants.BASE_API}/clocking/clocking-types");
            await foreach (var item in client.GetFromJsonAsAsyncEnumerable<ClockingType>(client.BaseAddress))
            {
                yield return item;
            }
        }

        public async Task<Response<Employee>> GetByIdAsync(string badgeId)
        {
            //handler = new HttpClientHandler
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            using var client = new HttpClient(handler);
            ///clocking/employee-info/{badgeId}
            client.BaseAddress = new Uri($"{Constants.BASE_API}/clocking/employee-info/{badgeId}");
            var response = await client.GetAsync(client.BaseAddress);
            if (response.IsSuccessStatusCode)
            {
                var employee = await response.Content.ReadFromJsonAsync<Response<Employee>>();
                return employee;
            }
            return new Response<Employee>
            {
                Success = false,
                Message = $"Error fetching employee info: {response.ReasonPhrase}"
            };
        }
    }
}
