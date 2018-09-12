using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace testpocetni.Models
{
    public class ProfilModel
    {
        public Korisnik Korisnik { get; set; }
        public dojmovi Dojam { get; set; }
        public List<dojmovi> ListaDojmova { get; set; }
        public List<rute> ListaRuta { get; set; }
    }
}