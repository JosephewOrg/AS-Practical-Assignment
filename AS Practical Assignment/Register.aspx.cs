using AS_Practical_Assignment.SITConnectServiceReference;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AS_Practical_Assignment
{
    public partial class Register : System.Web.UI.Page
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

        private bool ValidateName()
        {
            string fname = HttpUtility.HtmlEncode(tb_fname.Text.ToString().Trim());
            string lname = HttpUtility.HtmlEncode(tb_lname.Text.ToString().Trim());
            if (Regex.IsMatch(fname, @"^[\p{L}\p{M}' \.\-]+$") && Regex.IsMatch(lname, @"^[\p{L}\p{M}' \.\-]+$"))
            {
                return true;
            }
            return false;
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
            int score = CheckPasswordScore(tb_pwd.Text);
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

        protected bool IsCreditCardInfoValid(string cardNo, string cvv, string expiryDate)
        {
            var monthCheck = new Regex(@"^(0[1-9]|1[0-2])$");
            var yearCheck = new Regex(@"^20[0-9]{2}$");
            var cvvCheck = new Regex(@"^\d{3}$");

            if (Regex.IsMatch(cardNo, @"[^0-9 -]+") == true)
            {
                lbl_ccichecker.Text = "CardNo is invalid";
                return false;
            }
            else
            {
                string inString = Regex.Replace(cardNo, @"[^0-9]", "");
                if (inString.Length < 10)
                {
                    lbl_ccichecker.Text = "CardNo is invalid";
                    return false;
                }
                else
                {
                    char[] array = inString.ToCharArray();
                    Array.Reverse(array);
                    string reverse = new String(array);
                    int s1 = 0, s2 = 0;
                    for (int i = 0; i < reverse.Length; i++)
                    {
                        int digit;
                        if (int.TryParse(reverse[i].ToString(), out digit))
                        {
                            if (i % 2 == 0)
                            {
                                s1 += digit;
                            }
                            else
                            {
                                s2 += 2 * digit;
                                if (digit >= 5)
                                {
                                    s2 -= 9;
                                }
                            }
                        }
                        else
                        {
                            lbl_ccichecker.Text = "CardNo is invalid";
                            return false;
                        }
                    }
                }
            }

            if (!cvvCheck.IsMatch(cvv))
            {
                lbl_ccichecker.Text = "CVV is invalid";
                return false;
            }

            var dateParts = expiryDate.Split('/');          
            if (!monthCheck.IsMatch(dateParts[0]) || !yearCheck.IsMatch(dateParts[1]))
            {
                lbl_ccichecker.Text = "Expiry Date is invalid";
                return false;
            }

            var year = int.Parse(dateParts[1]);
            var month = int.Parse(dateParts[0]);
            var lastDateOfExpiryMonth = DateTime.DaysInMonth(year, month); //get actual expiry date
            var cardExpiry = new DateTime(year, month, lastDateOfExpiryMonth, 23, 59, 59);

            //check expiry greater than today & within next 6 years <7, 8>>
            return cardExpiry > DateTime.Now && cardExpiry < DateTime.Now.AddYears(6);
        }

        private bool ValidateInput()
        {
            int invalid = 0;

            if (!ValidateCaptcha())
            {
                lbl_fname.Text = "invalid captcha";
                invalid++;
            }

            if (!ValidateName())
            {
                lbl_fname.Text = "invalid names";
                invalid++;
            }

            if (!ValidateEmail())
            {
                lbl_fname.Text = "invalid email";
                invalid++;
            }

            if (!CheckPassword())
            {
                invalid++;
            }

            string cardno = HttpUtility.HtmlEncode(tb_cardno.Text.ToString().Trim());
            string cvv = HttpUtility.HtmlEncode(tb_cvv.Text.ToString().Trim());
            string expdate = HttpUtility.HtmlEncode(tb_expdate.Text.ToString().Trim());
            if (!IsCreditCardInfoValid(cardno, cvv, expdate))
            {
                invalid++;
            }

            if (invalid != 0)
            {
                return false;
            }

            return true;
        }

        protected void btn_register_Click(object sender, EventArgs e)
        {
            bool validInput = ValidateInput();
            if (validInput)
            {
                string fname = HttpUtility.HtmlEncode(tb_fname.Text.ToString().Trim());
                string lname = HttpUtility.HtmlEncode(tb_lname.Text.ToString().Trim());
                string cardno = HttpUtility.HtmlEncode(tb_cardno.Text.ToString().Trim());
                string cvv = HttpUtility.HtmlEncode(tb_cvv.Text.ToString().Trim());
                string expdate = HttpUtility.HtmlEncode(tb_expdate.Text.ToString().Trim());
                string email = HttpUtility.HtmlEncode(tb_email.Text.ToString().Trim());
                string pwd = HttpUtility.HtmlEncode(tb_pwd.Text.ToString().Trim());
                string dob = HttpUtility.HtmlEncode(tb_dob.Text.ToString().Trim());

                Service1Client client = new Service1Client();
                int result = client.CreateUser(email, fname, lname, cardno, cvv, expdate, pwd, dob);
                if (result == 0)
                {
                    lbl_msg.Text = "Email is already in use! Please use a different email.";
                    lbl_msg.ForeColor = Color.Red;
                }
                if (result == 1)
                {
                    lbl_msg.Text = "Your account has been successfully registered!";
                    lbl_msg.ForeColor = Color.Green;
                }

            }
            else
            {
                lbl_msg.Text = "Inputs are not valid!";
                lbl_msg.ForeColor = Color.Red;
            }
        }
    }
}