using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Semestro_projektas.Models
{
    public class Message
    {

        public int Id { get; set; }
        public int ChannelId { get; set; }
        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
        public Channel Channel { get; set; }
        public User Author { get; set; }
        public string Content { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;

    }
}
