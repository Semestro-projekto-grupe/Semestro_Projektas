using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Semestro_projektas.Models
{
    public class Message
    {

        public int Id { get; set; }

        public string Author { get; set; }
        public string Content { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;

    }
}
