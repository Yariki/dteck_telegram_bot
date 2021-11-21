using System.Net.Http.Json;
using DtekShutdownCheckBot.Shared.Entities;

namespace DtekShutdownCheckBotUIClient.Clients
{
    
    public class ChatClient
    {
        private readonly HttpClient _httpClient;

        public ChatClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Chat>> GetAllAsync() =>
            await _httpClient.GetFromJsonAsync<IEnumerable<Chat>>("api/chat/all");


        public async Task<Chat> GetByIdAsync(int id) =>
            await _httpClient.GetFromJsonAsync<Chat>($"api/chat/{id}");
        
    }
}