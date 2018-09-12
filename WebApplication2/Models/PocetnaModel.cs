using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace testpocetni.Models
{
    public class PocetnaModel
    {
        public rute ruta { get; set; }
        public List<rute> ListaRuta { get; set; }
        public List<PomPodaci> ListaPomocnihPodataka { get; set; }
    }
}