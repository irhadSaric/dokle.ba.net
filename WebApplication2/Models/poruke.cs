namespace testpocetni.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("poruke")]
    public partial class poruke
    {
        [Key]
        public int idPoruke { get; set; }

        public int idPosiljaoca { get; set; }

        public int idPrimaoca { get; set; }

        public byte? procitana { get; set; }

        [StringLength(500)]
        public string sadrzaj { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan vrijemeSlanja { get; set; }
    }
}
