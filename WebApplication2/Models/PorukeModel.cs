using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace testpocetni.Models
{
    public class PorukeModel
    {
        public poruke poruka { get; set; }
        public List<poruke> listaPoruka { get; set; }
        public List<poruke> listaZaCitanje { get; set; }
        public List<Korisnik> listaKorisnika { get; set; }
        public int brojNeprocitanih { get; set; }
    }
}