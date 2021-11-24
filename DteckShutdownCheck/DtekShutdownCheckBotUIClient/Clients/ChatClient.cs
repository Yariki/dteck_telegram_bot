using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DtekShutdownCheckBot.Shared.Entities;
using DtekShutdownCheckBot.Shared.Models;

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


        public async Task SendMessage(long chatId, string message)
        {
            var model = new Message()
            {
                ChatId = chatId,
                Text = message
            };
            await _httpClient.PostAsJsonAsync("api/chat/send", model);
        }

        
    }
}