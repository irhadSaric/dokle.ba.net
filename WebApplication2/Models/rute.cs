namespace testpocetni.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("rute")]
    public partial class rute
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int idRute { get; set; }

        public int idVozaca { get; set; }

        [StringLength(500)]
        [DisplayName("Polazište")]
        public string polaziste { get; set; }

        [StringLength(500)]
        [DisplayName("Odredište")]
        public string odrediste { get; set; }

        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Datum polaska")]
        public DateTime datum { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        [DisplayName("Vrijeme polaska")]
        public TimeSpan vrijeme { get; set; }

        [Column(TypeName = "text")]
        [DisplayName("Drzava")]
        public string drzava { get; set; }

        [Column(TypeName = "text")]
        [DisplayName("Nacin placanja")]
        public string nacinPlacanja { get; set; }
    }
}
