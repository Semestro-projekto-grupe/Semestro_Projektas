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
        [Required(ErrorMessage = "Neįvestas slapyvardis!")]
        [MinLength(2, ErrorMessage = "Minimalus slapyvardzio ilgis 2 simboliai!")]
        [MaxLength(30, ErrorMessage = "Maksimalus slapyvardzio ilgis 30 simbolių!")]
        public string NickName { get; set; }
        [DisplayName("Vardas:")]
        [Required(ErrorMessage = "Neįvestas vardas!")]
        [MinLength(2, ErrorMessage = "Minimalus vardo ilgis 2 simboliai!")]
        [MaxLength(30, ErrorMessage = "Maksimalus vardo ilgis 30 simbolių!")]
        public string Name { get; set; }
        [DisplayName("Pavardė:")]
        [Required(ErrorMessage = "Neįvesta pavarde!")]
        [MinLength(2, ErrorMessage = "Minimalus pavardes ilgis 2 simboliai!")]
        [MaxLength(30, ErrorMessage = "Maksimalus pavardes ilgis 30 simbolių!")]
        public string Surname { get; set; }
        [DataType(DataType.Password)]
        [DisplayName("Slaptažodis:")]
        [Required(ErrorMessage = "Neįvestas slaptažodis!")]
        [MinLength(6, ErrorMessage = "Minimalus slaptažodžio ilgis 6 simboliai!")]
        [MaxLength(20, ErrorMessage = "Maksimalus slaptažodžio ilgis 20 simbolių!")]
        [NotMapped]
        public string Password { get; set; }
        [DataType(DataType.Date)]
        [DisplayName("Data:")]
        [Required]
        public DateTime Date { get; set; }
        [DisplayName("Avataras:")]
        public string Avatar { get; set; }
        public bool NameVisibility { get; set; }
        public bool SurnameVisibility { get; set; }
        public bool DateVisibility { get; set; }

        public ICollection<ChannelUser> channelUsers { get; set; }
    }
}