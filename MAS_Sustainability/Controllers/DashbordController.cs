using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using System.Data;

namespace MAS_Sustainability.Controllers
{
    public class DashbordController : Controller
    {
        public String SessionEmail = null;
        // GET: Dashbord
        public ActionResult Index()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("Login", "UserLogin");
            }
            else
            {
                DB dbConn = new DB();
                using (MySqlConnection mySqlCon = dbConn.DBConnection())
                {
                    mySqlCon.Open();
                    MySqlCommand mySqlCmd = mySqlCon.CreateCommand();
                    mySqlCmd.CommandType = System.Data.CommandType.Text;
                    mySqlCmd.CommandText = "SELECT UserName FROM users WHERE UserEmail = '" + Session["user"] + "'";
                    mySqlCmd.ExecuteNonQuery();


                    MySqlDataAdapter mySqlDa = new MySqlDataAdapter(mySqlCmd);
                    DataTable dt = new DataTable();
                    mySqlDa.Fill(dt);


                   UserLogin userLoginModel = new UserLogin();


                }
            }
            return View();
        }

        public String getSessionEmail()
        {
            return SessionEmail;
        }
    }
}