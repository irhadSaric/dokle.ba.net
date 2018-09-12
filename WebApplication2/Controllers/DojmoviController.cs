using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using testpocetni.Models;

namespace testpocetni.Controllers
{
    public class DojmoviController : Controller
    {
        [HttpPost]
        public ActionResult Dojam(dojmovi Dojam)
        {
            Dojam.idPosiljaoca = Convert.ToInt32(Session["id"]);
            string constr = ConfigurationManager.ConnectionStrings["ConnectionStringName"].ConnectionString;
            SqlConnection sqlcon = new SqlConnection(constr);
            if (sqlcon.State == ConnectionState.Closed)
            {
                sqlcon.Open();
            }
            //return RedirectToAction(Dojam.idPrimaoca.ToString(), "Korisnik");
            SqlCommand sqlcmd = new SqlCommand("dodajDojam", sqlcon);
            sqlcmd.CommandType = CommandType.StoredProcedure;
            sqlcmd.Parameters.AddWithValue("@idPosiljaoca", Dojam.idPosiljaoca);
            sqlcmd.Parameters.AddWithValue("@idPrimaoca", Dojam.idPrimaoca);
            sqlcmd.Parameters.AddWithValue("@dojam", Dojam.dojam);
            sqlcmd.ExecuteNonQuery();
            sqlcon.Close();
            return RedirectToAction("Profil/"+Dojam.idPrimaoca.ToString(), "Korisnik");
        }
    }
}