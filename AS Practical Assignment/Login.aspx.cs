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
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

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

        private bool ValidateEmail()
        {
            var emailCheck = new Regex(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");
            string email = HttpUtility.HtmlEncode(tb_email.Text.ToString().Trim());
            if (!emailCheck.IsMatch(email))
            {
                return false;
            }
            return true;
        }

        private bool CheckPassword()
        {
            int score;
            string pwd = HttpUtility.HtmlEncode(tb_pwd.Text.ToString().Trim());
            if (pwd.Length < 8)
            {
                return false;
            }
            else
            {
                score = 1;
            }

            if (Regex.IsMatch(pwd, "[a-z]"))
            {
                score++;
            }

            if (Regex.IsMatch(pwd, "[A-Z]"))
            {
                score++;
            }

            if (Regex.IsMatch(pwd, "[0-9]"))
            {
                score++;
            }

            if (Regex.IsMatch(pwd, "[^A-Za-z0-9]"))
            {
                score++;
            }

            if (score < 4)
            {
                return false;
            }
            return true;
        }

        private bool ValidateInput()
        {
            int invalid = 0;

            if (!ValidateCaptcha())
            {
                invalid++;
            }

            if (!ValidateEmail())
            {
                invalid++;
            }

            if (!CheckPassword())
            {
                invalid++;
                lbl_pwdchecker.Text = "Password did not meet complexity standards.";
            }

            if (invalid != 0)
            {
                return false;
            }

            return true;
        }

        protected void btn_login_Click(object sender, EventArgs e)
        {
            bool validInput = ValidateInput();
            if (validInput)
            {
                string email = HttpUtility.HtmlEncode(tb_email.Text.ToString().Trim());
                string pwd = HttpUtility.HtmlEncode(tb_pwd.Text.ToString().Trim());

                Service1Client client = new Service1Client();
                User usr = client.GetUserByEmail(email);
                if (usr == null)
                {
                    lbl_msg.Text = "Email or password is not valid. Please try again.";
                    lbl_msg.ForeColor = Color.Red;
                } else {
                    if (usr.LockStatus == 1)
                    {
                        client.AccountLockout(email);
                        usr = client.GetUserByEmail(email);
                    }
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
                                if (usr.LockStatus == 0)
                                {
                                    client.ResetAccountLockout(email, 0, 0, DateTime.Now);
                                    Session["AuthID"] = email;
                                    Session["AuthName"] = usr.FirstName + " " + usr.LastName;

                                    string guid = Guid.NewGuid().ToString();
                                    Session["AuthToken"] = guid;

                                    Response.Cookies.Add(new HttpCookie("AuthToken", guid));

                                    Response.Redirect("Default.aspx", false);
                                }
                                else
                                {
                                    lbl_msg.Text = "Account has been locked out. Please try again in a while.";
                                    lbl_msg.ForeColor = Color.Red;
                                }
                            }
                            else
                            {
                                int result = client.AccountLockout(email);
                                if (result == 1)
                                {
                                    lbl_msg.Text = "Email or password is not valid. Please try again.";
                                    lbl_msg.ForeColor = Color.Red;
                                }
                                else
                                {
                                    lbl_msg.Text = "Account has been locked out. Please try again in a while.";
                                    lbl_msg.ForeColor = Color.Red;
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
            else
            {
                lbl_msg.Text = "Email or password is not valid. Please try again.";
                lbl_msg.ForeColor = Color.Red;
            }
        }
    }
}