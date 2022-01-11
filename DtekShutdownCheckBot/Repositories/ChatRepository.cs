using System;
using DtekShutdownCheckBot.Data;
using DtekShutdownCheckBot.Models;
using DtekShutdownCheckBot.Shared.Entities;
using LiteDB;
using Microsoft.Extensions.Options;

namespace DtekShutdownCheckBot.Repositories
{
    public class ChatRepository : BaseRepository<int, Chat>
    {
        public ChatRepository(DatabaseContext context) : base(context)
        {
        }
        
        public override Chat GetById(int key, string include = null)
        {
            return GetBy(c => c.Id == key, include);
        }

        public override void Update(Chat entity)
        {
            var item = GetById(entity.Id);
            if (item == null)
            {
                throw new NullReferenceException(nameof(item));
            }

            item = entity;
            Set.Update(item);
        }
        
        public override void Delete(int key)
        {
            var item = GetById(key);
            if (item == null)
            {
                throw new NullReferenceException(nameof(item));
            }
            Set.Remove(item);
        }
    }
}