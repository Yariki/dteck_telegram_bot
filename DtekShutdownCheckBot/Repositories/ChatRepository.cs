using System;
using DtekShutdownCheckBot.Models;
using DtekShutdownCheckBot.Models.Entities;
using LiteDB;
using Microsoft.Extensions.Options;

namespace DtekShutdownCheckBot.Repositories
{
    public class ChatRepository : BaseRepository<string, Chat>
    {
        public ChatRepository(LiteDatabase db) : base(db)
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