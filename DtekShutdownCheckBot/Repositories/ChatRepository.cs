using System;
using DtekShutdownCheckBot.Models;
using DtekShutdownCheckBot.Models.Entities;
using Microsoft.Extensions.Options;

namespace DtekShutdownCheckBot.Repositories
{
    public class ChatRepository : BaseRepository<string, Chat>
    {
        public ChatRepository(IOptions<LiteDbOptions> options) : base(options)
        {
        }
        
        public override Chat GetById(string key)
        {
            return GetBy(c => c.Id == key);
        }

        public override void Delete(string key)
        {
            Set.Delete(key);
        }
    }
}