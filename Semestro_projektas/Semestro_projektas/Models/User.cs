using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Semestro_projektas.Models
{
    public class User : IdentityUser
    {
        [DisplayName("Slapyvardis:")]
        [Required]
        public string NickName { get; set; }
        [DisplayName("Vardas:")]
        [Required]
        public string Name { get; set; }
        [DisplayName("Pavardė:")]
        [Required]
        public string Surname { get; set; }
        [DataType(DataType.Password)]
        [DisplayName("Slaptažodis:")]
        [Required(ErrorMessage = "Neįvestas slaptažodis!")]
        [MinLength(6, ErrorMessage = "Minimalus slaptažodžio ilgis 6 simboliai!")]
        [MaxLength(20, ErrorMessage = "Maksimalus slaptažodžio ilgis 20 simbolių!")]
        [NotMapped]
        public string Password { get; set; }
        [DataType(DataType.Date)]
        [DisplayName("Data")]
        [Required]
        public DateTime Date { get; set; }
        [DisplayName("Avataras:")]
        public string Avatar { get; set; }

        public ICollection<ChannelUser> channelUsers { get; set; }
    }
}
