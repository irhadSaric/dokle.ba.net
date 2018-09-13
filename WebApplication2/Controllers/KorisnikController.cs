using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using testpocetni.Models;

namespace testpocetni.Controllers
{
    public class KorisnikController : Controller
    {
        // GET: Korisnik
        [HttpGet]
        public ActionResult AddOrEdit()
        {
            Korisnik korisnikModel = new Korisnik();
            return View(korisnikModel);
        }

        [HttpPost]
        public ActionResult AddOrEdit(Korisnik korisnikModel)
        {
            using (korisnik kor = new korisnik())
            {
                if( kor.korisnicis.Any(x => x.email == korisnikModel.email))
                {
                    ViewBag.DuplicateMessage = "E-mail je vec u upotrebi";
                    return View("AddOrEdit", korisnikModel);
                }
                else
                {
                    Random rand = new Random();
                    string to = korisnikModel.email;
                    string from = "irhad.saric@hotmail.com";
                    string subject = "Aktivacijski kod";
                    int aktivacijskiKod = rand.Next(1000, 9999);
                    string text = aktivacijskiKod.ToString();
                    korisnikModel.aktivacijskiKod = aktivacijskiKod;
                    korisnikModel.profilna = "~/Content/profilne/default.jpg";
                    korisnikModel.aktivan = "nije";
                    try
                    {
                        MailMessage message = new MailMessage(from, to, subject, text);
                        
                        message.IsBodyHtml = false;

                        SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                        client.EnableSsl = true;
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.UseDefaultCredentials = false;
                        client.Credentials = new System.Net.NetworkCredential("doklebaa@gmail.com", "dokle.ba");
                        client.Send(message);
                        kor.korisnicis.Add(korisnikModel);
                        kor.SaveChanges();
                        Session["email"] = korisnikModel.email.ToString();
                        ModelState.Clear();
                        ViewBag.SuccessMessage = "Uspješno registrovan korisnik";
                        //return RedirectToAction("Hahu", "Aktivacija");
                        return Redirect("/Aktivacija/Aktivacija");

                    }

                    catch (Exception ex)
                    {
                        ViewBag.DuplicateMessage = "Greska" + ex.ToString();
                    }
                }
                ModelState.Clear();
            }
            return View("AddOrEdit", new Korisnik());
        }

        [HttpGet]
        public ActionResult Profil(int id)
        {
            Korisnik korisnikModel = new Korisnik();
            List<dojmovi> listaDojmova = new List<dojmovi>();
            List<rute> listaRuta = new List<rute>();

            string constr = ConfigurationManager.ConnectionStrings["ConnectionStringName"].ConnectionString;
            SqlConnection sqlcon = new SqlConnection(constr);
            if (sqlcon.State == ConnectionState.Closed)
            {
                sqlcon.Open();
            }
            SqlCommand sqlcmd = new SqlCommand("getPodaciKorisnika", sqlcon);
            SqlDataReader reader;
            sqlcmd.CommandType = CommandType.StoredProcedure;
            sqlcmd.Parameters.AddWithValue("@id", id);
            reader = sqlcmd.ExecuteReader();
            
            while (reader.Read())
            {
                korisnikModel.id = reader.GetInt32(0);
                korisnikModel.ime = reader.GetString(1);
                korisnikModel.prezime = reader.GetString(2);
                korisnikModel.email = reader.GetString(3);
                korisnikModel.profilna = reader.GetString(7);
                if (!reader.IsDBNull(8))
                    korisnikModel.mjestoStanovanja = reader.GetString(8);
                if (!reader.IsDBNull(9))
                    korisnikModel.brojTelefona = reader.GetString(9);
            }
            reader.Close();

            sqlcmd = new SqlCommand("selectRute", sqlcon);
            sqlcmd.CommandType = CommandType.StoredProcedure;
            sqlcmd.Parameters.AddWithValue("@id", id);
            reader = sqlcmd.ExecuteReader();

            while (reader.Read())
            {
                rute ruteModel = new rute();
                ruteModel.idRute = reader.GetInt32(0);
                ruteModel.polaziste = reader.GetString(2);
                ruteModel.odrediste = reader.GetString(3);
                ruteModel.datum = reader.GetDateTime(4);
                ruteModel.vrijeme = reader.GetTimeSpan(5);
                ruteModel.drzava = reader.GetString(6);
                ruteModel.nacinPlacanja = reader.GetString(7);
                listaRuta.Add(ruteModel);
            }
            reader.Close();

            sqlcmd = new SqlCommand("selectDojmove", sqlcon);
            sqlcmd.CommandType = CommandType.StoredProcedure;
            sqlcmd.Parameters.AddWithValue("@id", korisnikModel.id);
            reader = sqlcmd.ExecuteReader();
            while (reader.Read())
            {
                dojmovi dojamModel = new dojmovi();
                dojamModel.idPosiljaoca = reader.GetInt32(1);
                dojamModel.idPrimaoca = reader.GetInt32(2);
                dojamModel.dojam = reader.GetString(3);
                //reader.Close();
                SqlCommand sqlcmd2 = new SqlCommand("getPodaciKorisnika", sqlcon);
                SqlDataReader reader2;
                sqlcmd2.CommandType = CommandType.StoredProcedure;
                sqlcmd2.Parameters.AddWithValue("@id", reader.GetInt32(1));
                reader2 = sqlcmd2.ExecuteReader();

                while (reader2.Read())
                {
                    dojamModel.imePosiljaoca = reader2.GetString(1);
                    dojamModel.profilnaPosiljaoca = reader2.GetString(7);
                }
                reader2.Close();
                listaDojmova.Add(dojamModel);
            }
            sqlcon.Close();

            ProfilModel profilModel = new ProfilModel();
            dojmovi Dojam = new dojmovi();
            profilModel.Dojam = Dojam;
            profilModel.Korisnik = korisnikModel;
            profilModel.ListaDojmova = listaDojmova;
            profilModel.ListaRuta = listaRuta;
            return View(profilModel);
        }

        [HttpPost]
        public ActionResult Profil(ProfilModel korisnikModel, HttpPostedFileBase file)
        {
            //return Redirect("hehe"+korisnikModel.Korisnik.id.ToString());
            string constr = ConfigurationManager.ConnectionStrings["ConnectionStringName"].ConnectionString;
            SqlConnection sqlcon = new SqlConnection(constr);
            if (sqlcon.State == ConnectionState.Closed)
            {
                sqlcon.Open();
            }
            SqlCommand sqlcmd = new SqlCommand("dodajSliku", sqlcon);
            sqlcmd.CommandType = CommandType.StoredProcedure;
            sqlcmd.Parameters.AddWithValue("@id", Convert.ToInt32(korisnikModel.Korisnik.id));
            if (file != null && file.ContentLength > 0)
            {
                string imgpath = Path.Combine(Server.MapPath("~/Content/profilne/" + korisnikModel.Korisnik.id.ToString()) + Path.GetExtension(file.FileName));
                file.SaveAs(imgpath);
            }
            sqlcmd.Parameters.AddWithValue("@nova", "~/Content/profilne/" + korisnikModel.Korisnik.id.ToString() + Path.GetExtension(file.FileName));
            sqlcmd.ExecuteNonQuery();
            sqlcon.Close();
            korisnikModel.Korisnik.profilna = "~/Content/profilne/" + korisnikModel.Korisnik.id.ToString() + Path.GetExtension(file.FileName);

            //return View(korisnikModel);
            return RedirectToAction("Profil/"+korisnikModel.Korisnik.id.ToString(), "Korisnik");
        }

        /*
        [HttpPost]
        public ActionResult Profil(Korisnik korisnikModel, HttpPostedFileBase file)
        {
            string constr = ConfigurationManager.ConnectionStrings["ConnectionStringName"].ConnectionString;
            SqlConnection sqlcon = new SqlConnection(constr);
            if (sqlcon.State == ConnectionState.Closed)
            {
                sqlcon.Open();
            }
            SqlCommand sqlcmd = new SqlCommand("dodajSliku", sqlcon);
            sqlcmd.CommandType = CommandType.StoredProcedure;
            sqlcmd.Parameters.AddWithValue("@id", korisnikModel.id);
            if (file != null && file.ContentLength > 0)
            {
                string imgpath = Path.Combine(Server.MapPath("~/Content/profilne/" + korisnikModel.id.ToString()) + Path.GetExtension(file.FileName));
                file.SaveAs(imgpath);
            }
            sqlcmd.Parameters.AddWithValue("@nova", "~/Content/profilne/" + korisnikModel.id.ToString() + Path.GetExtension(file.FileName));
            sqlcmd.ExecuteNonQuery();
            sqlcon.Close();
            korisnikModel.profilna = "~/Content/profilne/" + korisnikModel.id.ToString() + Path.GetExtension(file.FileName);

            return View(korisnikModel);
            //return RedirectToAction("Profil", "Korisnik");
        } 
        */

        [HttpGet]
        public ActionResult Svi()
        {
            string constr = ConfigurationManager.ConnectionStrings["ConnectionStringName"].ConnectionString;
            SqlConnection sqlcon = new SqlConnection(constr);
            if (sqlcon.State == ConnectionState.Closed)
            {
                sqlcon.Open();
            }
            SqlCommand sqlcmd = new SqlCommand("SelectKorisnike", sqlcon);
            SqlDataReader reader;
            sqlcmd.CommandType = CommandType.StoredProcedure;
            reader = sqlcmd.ExecuteReader();

            List<Korisnik> niz = new List<Korisnik>();
            
            while (reader.Read())
            {
                Korisnik korisnikModel = new Korisnik();
                korisnikModel.id = reader.GetInt32(0);
                korisnikModel.ime = reader.GetString(1);
                korisnikModel.prezime = reader.GetString(2);
                korisnikModel.email = reader.GetString(3);
                niz.Add(korisnikModel);     
            }

            ViewBag.Poruka = niz;
            return View(niz);
        }

        [HttpGet]
        public ActionResult Login()
        {
            Korisnik korisnikModel = new Korisnik();
            return View(korisnikModel);
        }

        [HttpPost]
        public ActionResult Login(Korisnik korisnikModel)
        {
            using (korisnik kor = new korisnik())
            {
                if (kor.korisnicis.Any(x => x.email == korisnikModel.email))
                {
                    var user = kor.korisnicis.First(x => x.email == korisnikModel.email);
                    if(user.password == korisnikModel.password)
                    {
                        Session["id"] = user.id;

                        string constr = ConfigurationManager.ConnectionStrings["ConnectionStringName"].ConnectionString;
                        SqlConnection sqlcon = new SqlConnection(constr);
                        if (sqlcon.State == ConnectionState.Closed)
                        {
                            sqlcon.Open();
                        }

                        SqlCommand sqlcmd3 = new SqlCommand("ukupnoNeprocitanih", sqlcon);
                        sqlcmd3.CommandType = CommandType.StoredProcedure;
                        sqlcmd3.Parameters.AddWithValue("@id", Convert.ToInt32(Session["id"]));
                        SqlDataReader rd = sqlcmd3.ExecuteReader();
                        if (rd.HasRows)
                        {
                            rd.Read(); 
                            var brojUkupnoNeprocitanih = rd.GetInt32(0);
                            Session["brojUkupnoNeprocitanih"] = brojUkupnoNeprocitanih;
                        }
                        return Redirect("/Korisnik/Profil/" + user.id.ToString());
                    }
                    else
                    {
                        ViewBag.Greska = "Netačan password";
                        return View(korisnikModel);
                    }

                }
                else
                {
                    ViewBag.Greska = "Korisnik sa datom e-mail adresom ne postoji";
                    return View(korisnikModel);
                }
            }

            
        }

        [HttpGet]
        public ActionResult Ruta()
        {
            return View();
        }

        [HttpGet]
        public ActionResult LogOut()
        {
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
            return Redirect("/Korisnik/Login");
        }

        [HttpPost]
        public ActionResult Ruta(rute Ruta)
        {
            Ruta.idVozaca = Convert.ToInt32(Session["id"]);
            using (ruta ruta = new ruta())
            {
                try
                {
                    //Ruta.idRute = 2;
                    ruta.rutes.Add(Ruta);
                    ruta.SaveChanges();
                    ModelState.Clear();
                    ViewBag.SuccessMessage = "Uspješno registrovan korisnik";
                    //return RedirectToAction("Hahu", "Aktivacija");
                    return Redirect("/Korisnik/Profil/"+Ruta.idVozaca);

                }

                catch (Exception ex)
                {
                    ViewBag.Greska = ex.ToString();
                    return View();
                }
            }
        }

        [HttpPost]
        public ActionResult Update(Korisnik korisnik)
        {
            if(korisnik.brojTelefona != null)
            {
                using (korisnik kor = new korisnik())
                {
                    var result = kor.korisnicis.SingleOrDefault(b => b.id == korisnik.id);

                    if (result != null)
                    {
                        result.brojTelefona = korisnik.brojTelefona;
                        kor.SaveChanges();
                    }
                }
            }

            if (korisnik.mjestoStanovanja != null)
            {
                using (korisnik kor = new korisnik())
                {
                    var result = kor.korisnicis.SingleOrDefault(b => b.id == korisnik.id);

                    if (result != null)
                    {
                        result.mjestoStanovanja = korisnik.mjestoStanovanja;
                        kor.SaveChanges();
                    }
                }
            }

            return RedirectToAction("Profil/"+korisnik.id.ToString(), "Korisnik");
        }

        [HttpGet]
        public ActionResult Poruke(int id)
        {
            PorukeModel porukeModel = new PorukeModel();
            List<poruke> listaPoruka = new List<poruke>();
            List<int> listaIntova = new List<int>();
            List<Korisnik> listaKorisnika = new List<Korisnik>();
            List<poruke> listaZaCitanje = new List<poruke>();
            poruke Poruka = new poruke();
            Poruka.idPrimaoca = Convert.ToInt32(Session["id"]);
            Poruka.idPosiljaoca = Convert.ToInt32(id);

            string constr = ConfigurationManager.ConnectionStrings["ConnectionStringName"].ConnectionString;
            SqlConnection sqlcon = new SqlConnection(constr);
            if (sqlcon.State == ConnectionState.Closed)
            {
                sqlcon.Open();
            }

            SqlCommand sqlcmd4 = new SqlCommand("procitajPoruku", sqlcon);
            sqlcmd4.CommandType = CommandType.StoredProcedure;
            sqlcmd4.Parameters.AddWithValue("@idPosiljaoca", Poruka.idPosiljaoca);
            sqlcmd4.Parameters.AddWithValue("@idPrimaoca", Poruka.idPrimaoca);
            sqlcmd4.ExecuteNonQuery();

            SqlCommand sqlcmd5 = new SqlCommand("ukupnoNeprocitanih", sqlcon);
            sqlcmd5.CommandType = CommandType.StoredProcedure;
            sqlcmd5.Parameters.AddWithValue("@id", Convert.ToInt32(Session["id"]));
            SqlDataReader rd = sqlcmd5.ExecuteReader();
            if (rd.HasRows)
            {
                rd.Read();
                var brojUkupnoNeprocitanih2 = rd.GetInt32(0);
                Session["brojUkupnoNeprocitanih"] = brojUkupnoNeprocitanih2;
            }

            SqlCommand sqlcmd3 = new SqlCommand("ukupnoNeprocitanih", sqlcon);
            sqlcmd3.CommandType = CommandType.StoredProcedure;
            sqlcmd3.Parameters.AddWithValue("@id", Convert.ToInt32(Session["id"]));
            int brojUkupnoNeprocitanih = sqlcmd3.ExecuteNonQuery();

            SqlCommand sqlcmd = new SqlCommand("selectMojePoruke", sqlcon);
            SqlDataReader reader;
            sqlcmd.CommandType = CommandType.StoredProcedure;
            sqlcmd.Parameters.AddWithValue("@id", Convert.ToInt32(Session["id"]));
            reader = sqlcmd.ExecuteReader();

            while (reader.Read())
            {
                poruke porukeM = new poruke();

                porukeM.idPoruke = reader.GetInt32(0);
                porukeM.idPosiljaoca = reader.GetInt32(1);
                porukeM.idPrimaoca = reader.GetInt32(2);
                porukeM.procitana = reader.GetByte(3);
                porukeM.sadrzaj = reader.GetString(4);
                porukeM.vrijemeSlanja = reader.GetTimeSpan(5);

                if(!listaIntova.Contains(porukeM.idPosiljaoca))
                {
                    listaIntova.Add(porukeM.idPosiljaoca);
                    Korisnik korisnikModel = new Korisnik();
                    SqlCommand sqlcmd2 = new SqlCommand("getPodaciKorisnika", sqlcon);
                    SqlDataReader reader2;
                    sqlcmd2.CommandType = CommandType.StoredProcedure;
                    sqlcmd2.Parameters.AddWithValue("@id", porukeM.idPosiljaoca);
                    reader2 = sqlcmd2.ExecuteReader();

                    while (reader2.Read())
                    {
                        korisnikModel.id = reader2.GetInt32(0);
                        korisnikModel.ime = reader2.GetString(1);
                        korisnikModel.prezime = reader2.GetString(2);
                        korisnikModel.email = reader2.GetString(3);
                        korisnikModel.profilna = reader2.GetString(7);
                        if (!reader2.IsDBNull(8))
                            korisnikModel.mjestoStanovanja = reader2.GetString(8);
                        if (!reader2.IsDBNull(9))
                            korisnikModel.brojTelefona = reader2.GetString(9);

                        listaKorisnika.Add(korisnikModel);
                    }
                }

                listaPoruka.Add(porukeM);
            }

            if (id == Convert.ToInt32(Session["id"]))
            {
                
                
            }
            else
            {
                sqlcmd = new SqlCommand("selectPoruke", sqlcon);
                sqlcmd.CommandType = CommandType.StoredProcedure;
                sqlcmd.Parameters.AddWithValue("@idPrimaoca", Convert.ToInt32(Session["id"]));
                sqlcmd.Parameters.AddWithValue("@idPosiljaoca", id);
                reader = sqlcmd.ExecuteReader();

                while (reader.Read())
                {
                    poruke porukeM = new poruke();
                    porukeM.idPoruke = reader.GetInt32(0);
                    porukeM.idPosiljaoca = reader.GetInt32(1);
                    porukeM.idPrimaoca = reader.GetInt32(2);
                    porukeM.procitana = reader.GetByte(3);
                    porukeM.sadrzaj = reader.GetString(4);
                    porukeM.vrijemeSlanja = reader.GetTimeSpan(5);

                    listaZaCitanje.Add(porukeM);
                }
            }

            porukeModel.listaPoruka = listaPoruka;
            porukeModel.poruka = Poruka;
            porukeModel.brojNeprocitanih = brojUkupnoNeprocitanih;
            //Session["brojUkupnoNeprocitanih"] = brojUkupnoNeprocitanih;
            porukeModel.listaZaCitanje = listaZaCitanje;
            porukeModel.listaKorisnika = listaKorisnika;
            return View(porukeModel);
        }

        [HttpPost]
        public ActionResult Poruke(PorukeModel porukaModel)
        {
            porukaModel.poruka.vrijemeSlanja = DateTime.Now.TimeOfDay;
            var pom = porukaModel.poruka.idPosiljaoca;
            porukaModel.poruka.idPosiljaoca = Convert.ToInt32(Session["id"]);
            porukaModel.poruka.idPrimaoca = pom;
            porukaModel.poruka.procitana = 0;

            string constr = ConfigurationManager.ConnectionStrings["ConnectionStringName"].ConnectionString;
            SqlConnection sqlcon = new SqlConnection(constr);
            if (sqlcon.State == ConnectionState.Closed)
            {
                sqlcon.Open();
            }
            SqlCommand sqlcmd = new SqlCommand("dodajPoruku", sqlcon);
            sqlcmd.CommandType = CommandType.StoredProcedure;
            sqlcmd.Parameters.AddWithValue("@idPrimaoca", Convert.ToInt32(porukaModel.poruka.idPrimaoca));
            sqlcmd.Parameters.AddWithValue("@idPosiljaoca", Convert.ToInt32(Session["id"]));
            sqlcmd.Parameters.AddWithValue("@tekst", porukaModel.poruka.sadrzaj);
            sqlcmd.Parameters.AddWithValue("@procitana", 0);
            sqlcmd.Parameters.AddWithValue("@vrijeme", porukaModel.poruka.vrijemeSlanja);
            sqlcmd.ExecuteNonQuery();

            porukaModel.listaZaCitanje.Add(porukaModel.poruka);
            return RedirectToAction("Poruke/" + porukaModel.poruka.idPrimaoca, "Korisnik");
        }
        
    }

}