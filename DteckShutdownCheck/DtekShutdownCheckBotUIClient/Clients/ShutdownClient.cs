using System.Net.Http.Json;
using DtekShutdownCheckBot.Shared.Entities;

namespace DtekShutdownCheckBotUIClient.Clients
{
    public class ShutdownClient
    {
        private readonly HttpClient _httpClient;

        public ShutdownClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Shutdown>> GetAllLAsync() =>
            await _httpClient.GetFromJsonAsync<IEnumerable<Shutdown>>("api/shutdown/all");

        public async Task<IEnumerable<Shutdown>> GetNotSentShutdowns() =>
            await _httpClient.GetFromJsonAsync<IEnumerable<Shutdown>>("api/shutdown/notsend");

    }
}
