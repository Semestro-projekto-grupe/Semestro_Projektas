using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Semestro_projektas.Models
{
    public class User
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
        [DisplayName("Slaptažodis:")]
        [Required(ErrorMessage = "Neįvestas slaptažodis!")]
        [MinLength(6, ErrorMessage = "Minimalus slaptažodžio ilgis 6 simboliai!")]
        [MaxLength(16, ErrorMessage = "Maksimalus slaptažodžio ilgis 20 simbolių!")]
        public string Password { get; set; }
        [DataType(DataType.Date)]
        [DisplayName("Data")]
        [Required]
        public DateTime Date { get; set; }
    }
}
