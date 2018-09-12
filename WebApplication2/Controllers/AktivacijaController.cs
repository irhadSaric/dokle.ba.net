using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace testpocetni.Controllers
{
    public class AktivacijaController : Controller
    {
        // GET: Aktivacija
        [HttpGet]
        public ActionResult Aktivacija(int id = 0)
        {            
            return View();
        }

        [HttpPost]
        public ActionResult Aktivacija()
        {
            String unos = Request.Form["unosKoda"];
            ViewBag.Sesija = unos;
            string constr = ConfigurationManager.ConnectionStrings["ConnectionStringName"].ConnectionString;
            SqlConnection sqlcon = new SqlConnection(constr);
            if (sqlcon.State == ConnectionState.Closed)
            {
                sqlcon.Open();
            }
            SqlCommand sqlcmd = new SqlCommand("selectAktivacijskiKod", sqlcon);
            SqlDataReader reader;
            sqlcmd.CommandType = CommandType.StoredProcedure;
            sqlcmd.Parameters.AddWithValue("@email", Session["email"]);
            reader = sqlcmd.ExecuteReader();
            reader.Read();

            if (reader[0].ToString().Equals(unos))
            {
                SqlCommand aktivirati = new SqlCommand("updateAktivan", sqlcon);
                aktivirati.CommandType = CommandType.StoredProcedure;
                aktivirati.Parameters.AddWithValue("@email", Session["email"]);
                reader.Close();
                aktivirati.ExecuteNonQuery();
                //getPodaciKorisnikaEmail
                SqlCommand sqlcmd2 = new SqlCommand("getPodaciKorisnikaEmail", sqlcon);
                SqlDataReader reader2;
                sqlcmd2.CommandType = CommandType.StoredProcedure;
                sqlcmd2.Parameters.AddWithValue("@email", Session["email"]);
                reader2 = sqlcmd2.ExecuteReader();
                int id = 1444;
                while (reader2.Read())
                {
                    id = reader2.GetInt32(0);
                }
                reader2.Close();
                Session["id"] = id;
                return Redirect("/Korisnik/Profil/"+id.ToString());
            }
            sqlcon.Close();

            return View();
        }
    }
}