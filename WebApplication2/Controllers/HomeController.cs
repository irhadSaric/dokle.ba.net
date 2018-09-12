using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using testpocetni.Models;

namespace WebApplication2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpGet]
        public ActionResult Pocetna()
        {
            PocetnaModel pocetnaModel = new PocetnaModel();
            List<rute> listaRuta = new List<rute>();
            List<PomPodaci> ListaPomocnihPodataka = new List<PomPodaci>();

            string constr = ConfigurationManager.ConnectionStrings["ConnectionStringName"].ConnectionString;
            SqlConnection sqlcon = new SqlConnection(constr);
            if (sqlcon.State == ConnectionState.Closed)
            {
                sqlcon.Open();
            }

            SqlCommand tCommand = new SqlCommand("SELECT * FROM rute", sqlcon);
            using (SqlDataReader reader = tCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    rute ruta = new rute();
                    ruta.idVozaca = Convert.ToInt32(reader[1]);
                    ruta.polaziste = reader[2].ToString();
                    ruta.odrediste = reader[3].ToString();
                    ruta.datum = Convert.ToDateTime(reader[4]);
                    ruta.vrijeme = reader.GetTimeSpan(5);
                    ruta.drzava = reader[6].ToString();
                    ruta.nacinPlacanja = reader[7].ToString();

                    SqlCommand sqlcmd2 = new SqlCommand("getPodaciKorisnika", sqlcon);
                    SqlDataReader reader2;
                    sqlcmd2.CommandType = CommandType.StoredProcedure;
                    sqlcmd2.Parameters.AddWithValue("@id", reader.GetInt32(1));
                    reader2 = sqlcmd2.ExecuteReader();

                    while (reader2.Read())
                    {
                        PomPodaci pom = new PomPodaci();
                        pom.imeVozaca = reader2.GetString(1);
                        pom.prezimeVozaca = reader2.GetString(2);
                        pom.profilnaVozaca = reader2.GetString(7);
                        ListaPomocnihPodataka.Add(pom);
                    }
                    listaRuta.Add(ruta);
                }
            }
            pocetnaModel.ListaRuta = listaRuta;
            pocetnaModel.ListaPomocnihPodataka = ListaPomocnihPodataka;
            sqlcon.Close();
            return View(pocetnaModel);

                /*
                tCommand.CommandText = "UPDATE players SET name = @name, score = @score, active = @active WHERE jerseyNum = @jerseyNum";

                tCommand.Parameters.Add(new SqlParameter("@name", System.Data.SqlDbType.VarChar).Value = "Smith, Steve");
                tCommand.Parameters.Add(new SqlParameter("@score", System.Data.SqlDbType.Int).Value = "42");
                tCommand.Parameters.Add(new SqlParameter("@active", System.Data.SqlDbType.Bit).Value = true);
                tCommand.Parameters.Add(new SqlParameter("@jerseyNum", System.Data.SqlDbType.Int).Value = "99");
                */

                tCommand.ExecuteNonQuery();
            
        }

        [HttpPost]
        public ActionResult Pocetna(PocetnaModel pocetnaModel)
        {
            string sql = "SELECT * FROM rute WHERE ";
            List<KeyValuePair<string, string>> hahu = new List<KeyValuePair<string, string>>();
            List<KeyValuePair<string, TimeSpan>> hahu2 = new List<KeyValuePair<string, TimeSpan>>();
            List<KeyValuePair<string, DateTime>> hahu3 = new List<KeyValuePair<string, DateTime>>();

            if (pocetnaModel.ruta.odrediste != null)
            {
                KeyValuePair<string, string> clan= new KeyValuePair<string, string>("odrediste", pocetnaModel.ruta.odrediste);
                hahu.Add(clan);
            }
            if (pocetnaModel.ruta.polaziste != null)
            {
                KeyValuePair<string, string> clan = new KeyValuePair<string, string>("polaziste", pocetnaModel.ruta.polaziste);
                hahu.Add(clan);
            }
            if (pocetnaModel.ruta.nacinPlacanja != null)
            {
                KeyValuePair<string, string> clan = new KeyValuePair<string, string>("nacinPlacanja", pocetnaModel.ruta.nacinPlacanja);
                hahu.Add(clan);
            }
            if (pocetnaModel.ruta.vrijeme != null)
            {
                KeyValuePair<string, TimeSpan> clan = new KeyValuePair<string, TimeSpan>("vrijeme", pocetnaModel.ruta.vrijeme);
                hahu2.Add(clan);
            }
            if (pocetnaModel.ruta.datum != null)
            {
                KeyValuePair<string, DateTime> clan = new KeyValuePair<string, DateTime>("datum", pocetnaModel.ruta.datum);
                hahu3.Add(clan);
            }

            if(hahu.Count != 0)
            {
                for(int i = 0; i < hahu.Count; i++)
                {
                    if(i == hahu.Count - 1)
                    {
                        sql += hahu[i].Key + " = @" + hahu[i].Key;
                    }
                    else
                    {
                        sql += hahu[i].Key + " = @" + hahu[i].Key + " AND ";
                    }
                }
            }

            if (hahu2.Count != 0)
            {
                for (int i = 0; i < hahu2.Count; i++)
                {
                    if (i == hahu2.Count - 1)
                    {
                        if(hahu.Count > 0)
                        {
                            sql += " AND " + hahu2[i].Key + " = @" + hahu2[i].Key;
                        }   
                        else
                        {
                            sql += hahu2[i].Key + " = @" + hahu2[i].Key + " AND ";
                        }
                    }
                }
            }

            if (hahu3.Count != 0)
            {
                for (int i = 0; i < hahu3.Count; i++)
                {
                    if (i == hahu3.Count - 1)
                    {
                        if(hahu.Count > 0)
                        {
                            sql += " AND " + hahu3[i].Key + " = @" + hahu3[i].Key;
                        }
                        else
                        {
                            sql += hahu3[i].Key + " = @" + hahu3[i].Key;
                        }
                    }
                }
            }

            PocetnaModel pocetnaModelPom = new PocetnaModel();
            List<rute> listaRuta = new List<rute>();
            List<PomPodaci> ListaPomocnihPodataka = new List<PomPodaci>();

            string constr = ConfigurationManager.ConnectionStrings["ConnectionStringName"].ConnectionString;
            SqlConnection sqlcon = new SqlConnection(constr);
            if (sqlcon.State == ConnectionState.Closed)
            {
                sqlcon.Open();
            }

            SqlCommand tCommand = new SqlCommand(sql, sqlcon);
            for(int i = 0; i < hahu.Count; i++)
            {
                SqlParameter p1 = new SqlParameter("@" + hahu[i].Key, System.Data.SqlDbType.VarChar);
                p1.Value = hahu[i].Value;
                tCommand.Parameters.Add(p1);
               // tCommand.Parameters.AddWithValue("@" + hahu[i].Key, hahu[i].Value);
            }
            for (int i = 0; i < hahu2.Count; i++)
            {
                SqlParameter p1 = new SqlParameter("@" + hahu2[i].Key, System.Data.SqlDbType.Time);
                p1.Value = hahu2[i].Value;
                tCommand.Parameters.Add(p1);
                //tCommand.Parameters.AddWithValue("@" + hahu2[i].Key, hahu2[i].Value);
               // tCommand.Parameters.Add(new SqlParameter("@" + hahu2[i].Key, System.Data.SqlDbType.Time).Value = hahu2[i].Value);
            }
            for (int i = 0; i < hahu3.Count; i++)
            {
                SqlParameter p1 = new SqlParameter("@" + hahu3[i].Key, System.Data.SqlDbType.Date);
                p1.Value = hahu3[i].Value;
                tCommand.Parameters.Add(p1);
                //tCommand.Parameters.AddWithValue("@" + hahu3[i].Key, hahu3[i].Value);
                //tCommand.Parameters.Add(new SqlParameter("@" + hahu3[i].Key, System.Data.SqlDbType.Date).Value = hahu3[i].Value);
            }
            
            using (SqlDataReader reader = tCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    rute ruta = new rute();
                    ruta.idVozaca = Convert.ToInt32(reader[1]);
                    ruta.polaziste = reader[2].ToString();
                    ruta.odrediste = reader[3].ToString();
                    ruta.datum = Convert.ToDateTime(reader[4]);
                    ruta.vrijeme = reader.GetTimeSpan(5);
                    ruta.drzava = reader[6].ToString();
                    ruta.nacinPlacanja = reader[7].ToString();

                    SqlCommand sqlcmd2 = new SqlCommand("getPodaciKorisnika", sqlcon);
                    SqlDataReader reader2;
                    sqlcmd2.CommandType = CommandType.StoredProcedure;
                    sqlcmd2.Parameters.AddWithValue("@id", reader.GetInt32(1));
                    reader2 = sqlcmd2.ExecuteReader();

                    while (reader2.Read())
                    {
                        PomPodaci pom = new PomPodaci();
                        pom.imeVozaca = reader2.GetString(1);
                        pom.prezimeVozaca = reader2.GetString(2);
                        pom.profilnaVozaca = reader2.GetString(7);
                        ListaPomocnihPodataka.Add(pom);
                    }

                    listaRuta.Add(ruta);
                }
            }
            pocetnaModelPom.ListaRuta = listaRuta;
            pocetnaModelPom.ListaPomocnihPodataka = ListaPomocnihPodataka;
            sqlcon.Close();

            return View(pocetnaModelPom);
        }
    }
}