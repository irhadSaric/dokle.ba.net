namespace testpocetni.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("dojmovi")]
    public partial class dojmovi
    {
        [Key]
        public int idDojma { get; set; }

        public int idPosiljaoca { get; set; }

        public int idPrimaoca { get; set; }

        public string imePosiljaoca { get; set; }

        public string profilnaPosiljaoca { get; set; }

        [StringLength(255)]
        [DisplayName("Dojam")]
        public string dojam { get; set; }
    }
}
