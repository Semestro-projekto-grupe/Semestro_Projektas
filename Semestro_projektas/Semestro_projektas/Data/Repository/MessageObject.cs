using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Semestro_projektas.Data.Repository
{
    public class MessageObject
    {

        public int Id { get; set; }
        public int ChannelId { get; set; }
        public string Author { get; set; }
        public string Content { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;

        public MessageObject(int id, int channelId, string author, string content, DateTime created) {
            Id = id;
            ChannelId = channelId;
            Author = author;
            Content = content;
            Created = created;
        }
    }
}
