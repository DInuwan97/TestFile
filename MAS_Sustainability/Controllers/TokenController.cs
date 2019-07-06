using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;

namespace MAS_Sustainability.Controllers
{
    public class TokenController : Controller
    {
        [HttpGet]
        // GET: Token
        public ActionResult Index()
        {
            if(Session["user"] == null)
            {
                return RedirectToAction("Login", "UserLogin");
            }

            DB dbConn = new DB();
            DataTable dtblTokens = new DataTable();
            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {
                mySqlCon.Open();
                string qry = "SELECT tka.TokenAuditID,tk.ProblemName,tk.Location,tk.AttentionLevel,usr.UserName FROM users usr,tokens tk, token_audit tka WHERE tk.TokenAuditID = tka.TokenAuditID AND tka.AddedUser = usr.UserEmail";  
                MySqlDataAdapter mySqlDA = new MySqlDataAdapter(qry,mySqlCon);
                mySqlDA.Fill(dtblTokens);

            }
            return View(dtblTokens);
        }

        // GET: Token/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Token/Create
        public ActionResult Add()
        {
            return View();
        }

        // POST: Token/Create
        [HttpPost]
        public ActionResult Add(Token tokenModel)
        {

            //Image 01
            string first_name_of_file = Path.GetFileNameWithoutExtension(tokenModel.FirstImageFile.FileName);
            string extension1 = Path.GetExtension(tokenModel.FirstImageFile.FileName);
            first_name_of_file = first_name_of_file + DateTime.Now.ToString("yymmssffff") + extension1;
            tokenModel.FirstImagePath = "~/Image/" + first_name_of_file;
            first_name_of_file = Path.Combine(Server.MapPath("~/Image/"), first_name_of_file);
            tokenModel.FirstImageFile.SaveAs(first_name_of_file);

            String imgPath1 = null;
            imgPath1 = first_name_of_file;


            //Image 02
            string second_name_of_file = Path.GetFileNameWithoutExtension(tokenModel.SecondImageFile.FileName);
            string extension2 = Path.GetExtension(tokenModel.SecondImageFile.FileName);
            second_name_of_file = second_name_of_file + DateTime.Now.ToString("yymmssffff") + extension2;
            tokenModel.SecondImagePath = "~/Image/" + first_name_of_file;
            second_name_of_file = Path.Combine(Server.MapPath("~/Image/"), second_name_of_file);
            tokenModel.SecondImageFile.SaveAs(second_name_of_file);

            String imgPath2 = null;
            imgPath2 = second_name_of_file;

              
            String AddedUser = Session["user"].ToString();

            DB dbConn = new DB();
            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {

                mySqlCon.Open();
                // String qry = "INSERT INTO token_audit(AddedUser,Category,AddedDate)VALUES(@AddedUser,@Category,NOW())";

                MySqlCommand mySqlCmd_TokenAudit = new MySqlCommand("Proc_Store_TokenAudit", mySqlCon);
                mySqlCmd_TokenAudit.CommandType = CommandType.StoredProcedure;
                mySqlCmd_TokenAudit.Parameters.AddWithValue("_Category", tokenModel.ProblemCategory);
                mySqlCmd_TokenAudit.Parameters.AddWithValue("_AddedUser", AddedUser);


                mySqlCmd_TokenAudit.ExecuteNonQuery();


                String last_audit_id_qry = "SELECT TokenAuditID FROM token_audit ORDER BY TokenAuditID DESC LIMIT 1";
                MySqlDataAdapter mySqlDA = new MySqlDataAdapter(last_audit_id_qry, mySqlCon);
                DataTable dt = new DataTable();
                mySqlDA.Fill(dt);

                int last_audit_id_num = Convert.ToInt32(dt.Rows[0][0]);

                string qry = "INSERT INTO tokens(TokenAuditID,ProblemName,Location,AttentionLevel,Description) VALUES(@TokenAuditID,@ProblemName,@Location,@AttentionLevel,@Description)";

                MySqlCommand mySqlCmd_tokenInfo = new MySqlCommand(qry, mySqlCon);
                mySqlCmd_tokenInfo.Parameters.AddWithValue("@TokenAuditID", last_audit_id_num);
                mySqlCmd_tokenInfo.Parameters.AddWithValue("@ProblemName", tokenModel.ProblemName);
                mySqlCmd_tokenInfo.Parameters.AddWithValue("@Location", tokenModel.Location);
                mySqlCmd_tokenInfo.Parameters.AddWithValue("@AttentionLevel", tokenModel.AttentionLevel);
                mySqlCmd_tokenInfo.Parameters.AddWithValue("@Description", tokenModel.Description);
                mySqlCmd_tokenInfo.ExecuteNonQuery();


                MySqlCommand mySqlCmd_ImageUpload = new MySqlCommand("Proc_Store_Images", mySqlCon);
                mySqlCmd_ImageUpload.CommandType = CommandType.StoredProcedure;
                mySqlCmd_ImageUpload.Parameters.AddWithValue("_TokenAuditID", last_audit_id_num);
                mySqlCmd_ImageUpload.Parameters.AddWithValue("_ImgPath1", imgPath1);
                mySqlCmd_ImageUpload.Parameters.AddWithValue("_ImgPath2", imgPath2);
                mySqlCmd_ImageUpload.ExecuteNonQuery();


            }
            // TODO: Add insert logic here
            return View();
        }

  

        // GET: Token/Edit/5
        public ActionResult Edit(int id)
        {
            /*Token tokenModel = new Token();
            DataTable dtblToken = new DataTable();
            DataTable dtblTokenImage = new DataTable();

            DB dbConn = new DB();
            using (MySqlConnection mySqlCon = dbConn.DBConnection())
            {
                mySqlCon.Open();

                String qry = "SELECT tka.TokenAuditID,tk.ProblemName,tk.Location,tk.AttentionLevel,usr.UserName FROM users usr,tokens tk, token_audit tka WHERE tk.TokenAuditID = tka.TokenAuditID AND tka.AddedUser = usr.UserEmail AND tk.TokenAuditID = @TokenAuditID ";
                MySqlDataAdapter mySqlDa = new MySqlDataAdapter(qry, mySqlCon);
                mySqlDa.SelectCommand.Parameters.AddWithValue("@TokenAuditID", id);
                mySqlDa.Fill(dtblToken);


                String qryImg = "SELECT ImagePath FROM token_image WHERE TokenID = @TokenID";
                MySqlDataAdapter mySqlDaImg = new MySqlDataAdapter(qryImg, mySqlCon);
                mySqlDaImg.SelectCommand.Parameters.AddWithValue("@TokenID", id);
                mySqlDaImg.Fill(dtblTokenImage);


            }

            if (dtblToken.Rows.Count == 1)
            {
                //tokenModel.ProblemName = dtblToken.Rows[0][1].ToString();
                //tokenModel.Location = dtblToken.Rows[0][2].ToString();
               // tokenModel.AttentionLevel = Convert.ToInt32(dtblToken.Rows[0][3].ToString());
               // tokenModel.Description = dtblToken.Rows[0][5].ToString();

                return View(tokenModel);

            }
           
            else
            {
                return RedirectToAction("index");
            }
          
                if(dtblTokenImage.Rows.Count == 1)
                {
                   // tokenModel.FirstImagePath = dtblTokenImage.Rows[0][2].ToString();
                }

                if(dtblTokenImage.Rows.Count == 2)
                {
                   // tokenModel.FirstImagePath = dtblTokenImage.Rows[0][2].ToString();
                    //tokenModel.SecondImagePath = dtblTokenImage.Rows[1][2].ToString();
                }

            */
            return View();

        }

        // POST: Token/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Token/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Token/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
