using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SITConnectServiceReference.Entity
{
    public class User
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CardNo { get; set; }
        public string Cvv { get; set; }
        public string ExpiryDate { get; set; }
        public string Password { get; set; }
        public string DateOfBirth { get; set; }
        public string Salt { get; set; }
        public byte[] IV { get; set; }
        public byte[] Key { get; set; }
        public int LockStatus { get; set; }

        public User()
        {

        }

        public User(string email, string firstname, string lastname, string cardno, string cvv, string expirydate, string password, string dateofbirth, string salt, int lockstatus)
        {
            Email = email;
            FirstName = firstname;
            LastName = lastname;
            CardNo = cardno;
            Cvv = cvv;
            ExpiryDate = expirydate;
            Password = password;
            DateOfBirth = dateofbirth;
            Salt = salt;
            LockStatus = lockstatus;
        }
        
        protected byte[] EncryptData(string data, byte[] iv, byte[] key)
        {
            byte[] cipherText;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged
                {
                    IV = iv,
                    Key = key
                };
                ICryptoTransform encryptTransform = cipher.CreateEncryptor();

                byte[] plainText = Encoding.UTF8.GetBytes(data);
                cipherText = encryptTransform.TransformFinalBlock(plainText, 0, plainText.Length);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return cipherText;
        }

        protected string DecryptData(byte[] cipherText, byte[] iv, byte[] key)
        {
            string plainText = null;
            try
            {
                RijndaelManaged cipher = new RijndaelManaged
                {
                    IV = iv,
                    Key = key
                };

                ICryptoTransform decryptTransform = cipher.CreateDecryptor();

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptTransform, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plainText = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return plainText;
        }

        protected bool CheckIfUserExists(string email)
        {
            try
            {
                string DBConnect = ConfigurationManager.ConnectionStrings["SITConnectDB"].ConnectionString;
                SqlConnection myConn = new SqlConnection(DBConnect);

                string sqlStmt = "Select * from Users Where email = @paraEmail";
                SqlDataAdapter da = new SqlDataAdapter(sqlStmt, myConn);
                da.SelectCommand.Parameters.AddWithValue("@paraEmail", email);

                DataSet ds = new DataSet();

                da.Fill(ds);

                int rec_cnt = ds.Tables[0].Rows.Count;
                if (rec_cnt == 1)
                {
                    return true;
                }
                return false;
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public int UpdateAccountLockout(string email, int lockstatus, int lockattempt, DateTime locktimestamp)
        {
            try
            {
                string DBConnect = ConfigurationManager.ConnectionStrings["SITConnectDB"].ConnectionString;
                SqlConnection myConn = new SqlConnection(DBConnect);

                string sqlStmt = "Update Users set lockStatus = @paraLockStatus, lockAttempt = @paraLockAttempt, lockTimeStamp = @paraLockTimeStamp where email = @paraEmail";
                SqlCommand sqlCmd = new SqlCommand(sqlStmt, myConn);
                sqlCmd.Parameters.AddWithValue("@paraLockStatus", lockstatus);
                sqlCmd.Parameters.AddWithValue("@paraLockAttempt", lockattempt);
                sqlCmd.Parameters.AddWithValue("@paraLockTimeStamp", locktimestamp);
                sqlCmd.Parameters.AddWithValue("@paraEmail", email);

                myConn.Open();
                int result = sqlCmd.ExecuteNonQuery();
                myConn.Close();

                return result;
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public int AccountLockout(string email)
        {
            try
            {
                string DBConnect = ConfigurationManager.ConnectionStrings["SITConnectDB"].ConnectionString;
                SqlConnection myConn = new SqlConnection(DBConnect);

                string sqlStmt = "Select * from Users Where email = @paraEmail";
                SqlDataAdapter da = new SqlDataAdapter(sqlStmt, myConn);
                da.SelectCommand.Parameters.AddWithValue("@paraEmail", email);

                DataSet ds = new DataSet();

                da.Fill(ds);

                int rec_cnt = ds.Tables[0].Rows.Count;
                if (rec_cnt == 1)
                {
                    DataRow row = ds.Tables[0].Rows[0];

                    int lockstatus;
                    int lockattempt;
                    DateTime locktimestamp;
                    TimeSpan ts = new TimeSpan(0, 5, 0);
                    if (Int32.TryParse(row["lockStatus"].ToString(), out int ls) == true && Int32.TryParse(row["lockAttempt"].ToString(), out int la) == true && DateTime.TryParse(row["lockTimeStamp"].ToString(), out DateTime lts) == true)
                    {
                        lockstatus = ls;
                        lockattempt = la;
                        locktimestamp = lts;
                        if (lockstatus == 1 && lockattempt == 3 && DateTime.Now - ts >= locktimestamp)
                        {
                            lockstatus = 0;
                            lockattempt = 0;
                            locktimestamp = DateTime.Now;
                        }
                        else
                        {
                            if (lockattempt >= 0 && lockattempt < 3)
                            {
                                lockattempt++;
                            }
                            else if (lockstatus == 0 && lockattempt == 3)
                            {
                                lockstatus = 1;
                                locktimestamp = DateTime.Now;

                                UpdateAccountLockout(email, lockstatus, lockattempt, locktimestamp);

                                return 0;
                            }
                        }
                    }
                    else
                    {
                        lockstatus = 0;
                        lockattempt = 1;
                        locktimestamp = DateTime.Now;
                    }

                    int result = UpdateAccountLockout(email, lockstatus, lockattempt, locktimestamp);

                    return result;
                }
                else
                {
                    return 0;
                }
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        // Not functional "Advanced Features"
        /* protected bool CheckPasswordPolicies(string email, string password)
        {
            try
            {
                string DBConnect = ConfigurationManager.ConnectionStrings["SITConnectDB"].ConnectionString;
                SqlConnection myConn = new SqlConnection(DBConnect);

                string sqlSelectStmt = "Select * from Users Where email = @paraEmail";
                SqlDataAdapter da = new SqlDataAdapter(sqlSelectStmt, myConn);
                da.SelectCommand.Parameters.AddWithValue("@paraEmail", email);

                DataSet ds = new DataSet();

                da.Fill(ds);

                int rec_cnt = ds.Tables[0].Rows.Count;
                if (rec_cnt == 1)
                {
                    DataRow row = ds.Tables[0].Rows[0];

                    byte[] iv = Convert.FromBase64String(row["iv"].ToString());
                    byte[] key = Convert.FromBase64String(row["key"].ToString());

                    string salt = DecryptData(Convert.FromBase64String(row["salt"].ToString()), iv, key);

                    SHA512Managed hashing = new SHA512Managed();

                    string passwordWithSalt = password + salt;
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(passwordWithSalt));
                    string passwordFinalHash = Convert.ToBase64String(hashWithSalt);

                    string passwordHistory1 = row["passwordHistory1"].ToString();
                    string passwordHistory2 = row["passwordHistory2"].ToString();
                    if (passwordHistory1 != null && passwordHistory1 != "")
                    {
                        passwordHistory1 = DecryptData(Convert.FromBase64String(row["passwordHistory1"].ToString()), iv, key);
                    }

                    if (passwordHistory2 != null && passwordHistory2 != "")
                    {
                        passwordHistory2 = DecryptData(Convert.FromBase64String(row["passwordHistory2"].ToString()), iv, key);
                    }

                    DateTime passwordTimeStamp;
                    TimeSpan ts = new TimeSpan(0, 5, 0);
                    if (DateTime.TryParse(row["passwordTimeStamp"].ToString(), out DateTime pts) == true)
                    {
                        passwordTimeStamp = pts;
                        if (DateTime.Now - ts >= passwordTimeStamp)
                        {
                            if (passwordFinalHash != passwordHistory1 && passwordFinalHash != passwordHistory2)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return false;
                        }

                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.ToString());
            }
            finally { }
            return false;
        }

        public int UpdatePassword(string email, string password)
        {
            if (!CheckPasswordPolicies(email, password))
            {
                try
                {
                    string DBConnect = ConfigurationManager.ConnectionStrings["SITConnectDB"].ConnectionString;
                    SqlConnection myConn = new SqlConnection(DBConnect);

                    string sqlSelectStmt = "Select * from Users Where email = @paraEmail";
                    SqlDataAdapter da = new SqlDataAdapter(sqlSelectStmt, myConn);
                    da.SelectCommand.Parameters.AddWithValue("@paraEmail", email);

                    DataSet ds = new DataSet();

                    da.Fill(ds);

                    int rec_cnt = ds.Tables[0].Rows.Count;
                    if (rec_cnt == 1)
                    {
                        DataRow row = ds.Tables[0].Rows[0];

                        byte[] iv = Convert.FromBase64String(row["iv"].ToString());
                        byte[] key = Convert.FromBase64String(row["key"].ToString());

                        string salt = DecryptData(Convert.FromBase64String(row["salt"].ToString()), iv, key);

                        SHA512Managed hashing = new SHA512Managed();

                        string passwordWithSalt = password + salt;
                        byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(passwordWithSalt));
                        string passwordFinalHash = Convert.ToBase64String(hashWithSalt);

                        string oldPassword = DecryptData(Convert.FromBase64String(row["password"].ToString()), iv, key);

                        string passwordHistory1 = row["passwordHistory1"].ToString();
                        string passwordHistory2 = row["passwordHistory2"].ToString();

                        string newEncryptedPasswordHistory2 = null;

                        if (passwordHistory1 != null && passwordHistory1 != "")
                        {
                            passwordHistory1 = DecryptData(Convert.FromBase64String(row["passwordHistory1"].ToString()), iv, key);
                            newEncryptedPasswordHistory2 = Convert.ToBase64String(EncryptData(passwordHistory1, iv, key));
                        }

                        if (passwordHistory2 != null && passwordHistory2 != "")
                        {
                            passwordHistory2 = DecryptData(Convert.FromBase64String(row["passwordHistory2"].ToString()), iv, key);
                        }

                        string sqlStmt = "Update Users set password = @paraPassword, passwordTimeStamp = @paraPasswordTimeStamp, passwordHistory1 = @paraPasswordHistory1, passwordHistory2 = @paraPasswordHistory2 where email = @paraEmail";

                        string encryptedPassword = Convert.ToBase64String(EncryptData(passwordFinalHash, iv, key));
                        string newEncryptedPasswordHistory1 = Convert.ToBase64String(EncryptData(oldPassword, iv, key));

                        SqlCommand sqlCmd = new SqlCommand(sqlStmt, myConn);
                        sqlCmd.Parameters.AddWithValue("@paraPassword", encryptedPassword);
                        sqlCmd.Parameters.AddWithValue("@paraPasswordTimeStamp", DateTime.Now);
                        sqlCmd.Parameters.AddWithValue("@paraPasswordHistory1", newEncryptedPasswordHistory1);
                        sqlCmd.Parameters.AddWithValue("@paraPasswordHistory2", newEncryptedPasswordHistory2);
                        sqlCmd.Parameters.AddWithValue("@paraEmail", email);

                        myConn.Open();
                        int result = sqlCmd.ExecuteNonQuery();
                        myConn.Close();

                        return result;
                    }
                    else
                    {
                        return 0;
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception(ex.ToString());
                }
            }
            else
            {
                return 0;
            }

        }*/

        public int Insert(string email, string firstname, string lastname, string cardno, string cvv, string expirydate, string password, string dateofbirth)
        {
            if (!CheckIfUserExists(email))
            {
                // Hash & Encrypt sensitive information
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                byte[] saltByte = new byte[8];

                rng.GetBytes(saltByte);
                string salt = Convert.ToBase64String(saltByte);
                SHA512Managed hashing = new SHA512Managed();

                string passwordWithSalt = password + salt;
                byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(passwordWithSalt));
                string passwordFinalHash = Convert.ToBase64String(hashWithSalt);
                RijndaelManaged cipher = new RijndaelManaged();
                cipher.GenerateKey();
                byte[] iv = cipher.IV;
                byte[] key = cipher.Key;

                string encryptedPasswordHash = Convert.ToBase64String(EncryptData(passwordFinalHash, iv, key));
                string encryptedCardNo = Convert.ToBase64String(EncryptData(cardno, iv, key));
                string encryptedCvv = Convert.ToBase64String(EncryptData(cvv, iv, key));
                string encryptedExpiryDate = Convert.ToBase64String(EncryptData(expirydate, iv, key));
                string encryptedSalt = Convert.ToBase64String(EncryptData(salt, iv, key));

                try
                {
                    // Insert information into DB
                    string DBConnect = ConfigurationManager.ConnectionStrings["SITConnectDB"].ConnectionString;
                    SqlConnection myConn = new SqlConnection(DBConnect);

                    string sqlStmt = "Insert into Users (email, firstName, lastName, cardNo, cvv, expiryDate, password, passwordTimeStamp, dateOfBirth, salt, iv, [key]) VALUES(@paraEmail, @paraFirstName, @paraLastName, @paraCardNo, @paraCvv, @paraExpiryDate, @paraPassword, @paraPasswordTimeStamp, @paraDateOfBirth, @paraSalt, @paraIV, @paraKey)";
                    SqlCommand sqlCmd = new SqlCommand(sqlStmt, myConn);
                    sqlCmd.Parameters.AddWithValue("@paraEmail", email);
                    sqlCmd.Parameters.AddWithValue("@paraFirstName", firstname);
                    sqlCmd.Parameters.AddWithValue("@paraLastName", lastname);
                    sqlCmd.Parameters.AddWithValue("@paraCardNo", encryptedCardNo);
                    sqlCmd.Parameters.AddWithValue("@paraCvv", encryptedCvv);
                    sqlCmd.Parameters.AddWithValue("@paraExpiryDate", encryptedExpiryDate);
                    sqlCmd.Parameters.AddWithValue("@paraPassword", encryptedPasswordHash);
                    sqlCmd.Parameters.AddWithValue("@paraPasswordTimeStamp", DateTime.Now);
                    sqlCmd.Parameters.AddWithValue("@paraDateOfBirth", dateofbirth);
                    sqlCmd.Parameters.AddWithValue("@paraSalt", encryptedSalt);
                    sqlCmd.Parameters.AddWithValue("@paraIV", Convert.ToBase64String(iv));
                    sqlCmd.Parameters.AddWithValue("@paraKey", Convert.ToBase64String(key));

                    myConn.Open();
                    int result = sqlCmd.ExecuteNonQuery();
                    myConn.Close();

                    return result;
                }
                catch (SqlException ex)
                {
                    throw new Exception(ex.ToString());
                }
            } 
            else
            {
                return 0;
            }
        }

        public User SelectByEmail(string email)
        {
            try
            {
                string DBConnect = ConfigurationManager.ConnectionStrings["SITConnectDB"].ConnectionString;
                SqlConnection myConn = new SqlConnection(DBConnect);

                string sqlStmt = "Select * from Users Where email = @paraEmail";
                SqlDataAdapter da = new SqlDataAdapter(sqlStmt, myConn);
                da.SelectCommand.Parameters.AddWithValue("@paraEmail", email);

                DataSet ds = new DataSet();

                da.Fill(ds);

                User usr = null;
                int rec_cnt = ds.Tables[0].Rows.Count;
                if (rec_cnt == 1)
                {
                    DataRow row = ds.Tables[0].Rows[0];
                    string firstname = row["firstName"].ToString();
                    string lastname = row["lastName"].ToString();
                    string dateofbirth = row["dateOfBirth"].ToString();
                    int lockstatus = 0;
                    if (Int32.TryParse(row["lockStatus"].ToString(), out int ls) == true)
                    {
                        lockstatus = ls;
                    }

                    // Retrieve IV & Key
                    byte[] iv = Convert.FromBase64String(row["iv"].ToString());
                    byte[] key = Convert.FromBase64String(row["key"].ToString());

                    // Decrypt ciphertext
                    string cardno = DecryptData(Convert.FromBase64String(row["cardNo"].ToString()), iv, key);
                    string cvv = DecryptData(Convert.FromBase64String(row["cvv"].ToString()), iv, key);
                    string expirydate = DecryptData(Convert.FromBase64String(row["expiryDate"].ToString()), iv, key);
                    string password = DecryptData(Convert.FromBase64String(row["password"].ToString()), iv, key);   // Get pwdHashWithSalt
                    string salt = DecryptData(Convert.FromBase64String(row["salt"].ToString()), iv, key);           // Get Salt

                    usr = new User(email, firstname, lastname, cardno, cvv, expirydate, password, dateofbirth, salt, lockstatus);
                }

                return usr;
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
