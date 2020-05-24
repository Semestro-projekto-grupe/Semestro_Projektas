using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Semestro_projektas.Models
{
    public class Channel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<ChannelUser> channelUsers { get; set; }
        public ICollection<Message> messages { get; set; }
    }
}

