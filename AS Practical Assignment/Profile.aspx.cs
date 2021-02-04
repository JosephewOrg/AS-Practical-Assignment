using AS_Practical_Assignment.SITConnectServiceReference;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AS_Practical_Assignment
{
    public partial class Profile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["AuthID"] != null && Session["AuthName"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
            {
                if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                {
                    Response.Redirect("Login.aspx", false);
                }
                else
                {
                    string userID = (string)Session["AuthID"];
                    Service1Client client = new Service1Client();
                    User usr = client.GetUserByEmail(userID);
                    lbl_username.Text = (string)Session["AuthName"];
                    lbl_email.Text = userID;
                    lbl_dob.Text = usr.DateOfBirth;
                }
            }
            else
            {
                Response.Redirect("Login.aspx", false);
            }
        }

        protected void btn_logout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();

            Response.Redirect("Login.aspx", false);

            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
            }

            if (Request.Cookies["AuthToken"] != null)
            {
                Response.Cookies["AuthToken"].Value = string.Empty;
                Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
            }
        }

        public class MyObject
        {
            public string success { get; set; }
            public DateTime challenge_ts { get; set; }
            public string hostname { get; set; }
        }

        public bool ValidateCaptcha()
        {
            bool result = true;

            // When user submits the recaptcha form, the user gets a response POST parameter.
            // captchaResponse consists of the user click pattern. Behaviour Analytics! AI :)
            string captchaResponse = Request.Form["g-recaptcha-response"];

            // To send a GET request to Google along with the response and Secret key.
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create
                ("https://www.google.com/recaptcha/api/siteverify?secret=&response=" + captchaResponse);

            try
            {
                // Codes to receive the Response in JSON format from Google Server
                using (WebResponse wResponse = req.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                    {
                        // The response in JSON format
                        string jsonResponse = readStream.ReadToEnd();

                        // To show the JSON response string for learning purpose
                        // lbl_gScore.Text = jsonResponse.ToString();

                        JavaScriptSerializer js = new JavaScriptSerializer();

                        // Create jsonObject to handle the response e.g. success or Error
                        // Deserialize Json
                        MyObject jsonObject = js.Deserialize<MyObject>(jsonResponse);

                        // Convert the string "False" to bool false or "True" to bool true
                        result = Convert.ToBoolean(jsonObject.success);
                    }
                }

                return result;
            }
            catch (WebException ex)
            {
                throw ex;
            }
        }

        private int CheckPasswordScore(string password)
        {
            int score;
            if (password.Length < 8)
            {
                return 1;
            }
            else
            {
                score = 1;
            }

            if (Regex.IsMatch(password, "[a-z]"))
            {
                score++;
            }

            if (Regex.IsMatch(password, "[A-Z]"))
            {
                score++;
            }

            if (Regex.IsMatch(password, "[0-9]"))
            {
                score++;
            }

            if (Regex.IsMatch(password, "[^A-Za-z0-9]"))
            {
                score++;
            }

            return score;
        }

        private bool CheckPassword()
        {
            string pwd = HttpUtility.HtmlEncode(tb_newpwd.Text.ToString().Trim());
            int score = CheckPasswordScore(pwd);
            string complexity = "";
            switch (score)
            {
                case 1:
                    complexity = "Very Weak";
                    break;

                case 2:
                    complexity = "Weak";
                    break;

                case 3:
                    complexity = "Medium";
                    break;

                case 4:
                    complexity = "Strong";
                    break;

                case 5:
                    complexity = "Excellent";
                    break;
            }
            lbl_pwdchecker.Text = $"Password Complexity - {complexity}";
            if (score < 4)
            {
                lbl_pwdchecker.ForeColor = Color.Red;
                return false;
            }
            lbl_pwdchecker.ForeColor = Color.Green;
            return true;
        }

        protected void btn_changepwd_Click(object sender, EventArgs e)
        {
            string pwd = HttpUtility.HtmlEncode(tb_newpwd.Text.ToString().Trim());
            string confirmpwd = HttpUtility.HtmlEncode(tb_confirmpwd.Text.ToString().Trim());
            if (ValidateCaptcha() && CheckPassword() && CheckPasswordScore(confirmpwd) > 3)
            {
                string email = (string)Session["AuthID"];
                Service1Client client = new Service1Client();
                User usr = client.GetUserByEmail(email);
                if (usr != null)
                {
                    string dbHash = usr.Password;
                    string dbSalt = usr.Salt;
                    SHA512Managed hashing = new SHA512Managed();
                    try
                    {
                        if (dbSalt != null && dbSalt.Length > 0 && dbHash != null && dbHash.Length > 0)
                        {
                            string pwdWithSalt = pwd + dbSalt;
                            byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                            string userHash = Convert.ToBase64String(hashWithSalt);
                            if (userHash.Equals(dbHash))
                            {
                                lbl_pwdchecker.Text = "Cannot change to the same password.";
                                lbl_pwdchecker.ForeColor = Color.Red;
                            }
                            else
                            {
                                if (pwd.Equals(confirmpwd))
                                {
                                    // Not functional
                                    // int result = client.ChangePassword(email, pwd);
                                    int result = 0;
                                    if (result == 1)
                                    {
                                        lbl_pwdchecker.Text = "Password has been successfully changed.";
                                        lbl_pwdchecker.ForeColor = Color.Green;
                                    }
                                    else
                                    {
                                        lbl_pwdchecker.Text = "Password did not meet the account policy requirements.";
                                        lbl_pwdchecker.ForeColor = Color.Red;
                                    }
                                }
                                else
                                {
                                    lbl_pwdchecker.Text = "Make sure both passwords are the same.";
                                    lbl_pwdchecker.ForeColor = Color.Red;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.ToString());
                    }
                    finally { }
                }
            }
        }
    }
}