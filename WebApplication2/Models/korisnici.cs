namespace testpocetni.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("korisnici")]
    public partial class Korisnik
    {
        /*public Korisnik(int id)
        {
            this.id = id;
        }
        
        public Korisnik()
        {

        }
        */

        public int id { get; set; }

        [Column(TypeName = "text")]
        [Required(ErrorMessage = "Obavezno polje")]
        [DisplayName("Ime")]
        public string ime { get; set; }

        [Column(TypeName = "text")]
        [Required(ErrorMessage = "Obavezno polje")]
        [DisplayName("Prezime")]
        public string prezime { get; set; }

        [Required(ErrorMessage = "Obavezno polje")]
        [StringLength(255)]
        [DisplayName("E-mail")]
        [EmailAddress(ErrorMessage = "Unesite validnu e-mail adresu")]
        public string email { get; set; }

        [Required(ErrorMessage = "Obavezno polje")]
        [StringLength(25)]
        [DisplayName("Password")]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [StringLength(25)]
        [DataType(DataType.Password)]
        [DisplayName("Confirm password")]
        [Compare("password", ErrorMessage = "Pasvordi se ne podudaraju")]
        public string confirmpassword { get; set; }


        public int aktivacijskiKod { get; set; }

        [StringLength(5)]
        public string aktivan { get; set; }

        [StringLength(255)]
        public string profilna { get; set; }

        [StringLength(400)]
        public string mjestoStanovanja { get; set; }

        [StringLength(15)]
        public string brojTelefona { get; set; }
    }
}
