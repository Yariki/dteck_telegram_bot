using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LiteDB;

namespace DtekShutdownCheckBot.Shared.Entities
{
    public class Chat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public long ChatId { get; set; }
        
        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
        
        public string Bio { get; set; }

        public string Description { get; set; }

        public string Title { get; set; }
        
        public virtual ICollection<Word> Words { get; set; }

    }
}