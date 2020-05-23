using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Semestro_projektas.Models
{
    public class ChannelUser
    {
        public int ChannelId { get; set; }
        public Channel Channel { get; set; }
        public string UserId { get; set; }
       // [ForeignKey("UserId")]
        public User User { get; set; }
        public RoleTypes Role { get; set; }
    }

}
