using SITConnectServiceReference.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SITConnectServiceReference
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class Service1 : IService1
    {
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        public int ResetAccountLockout(string email, int lockstatus, int lockattempt, DateTime locktimestamp)
        {
            User usr = new User();
            return usr.UpdateAccountLockout(email, lockstatus, lockattempt, locktimestamp);
        }

        public int AccountLockout(string email)
        {
            User usr = new User();
            return usr.AccountLockout(email);
        }

        public int CreateUser(string email, string firstname, string lastname, string cardno, string cvv, string expirydate, string password, string dateofbirth)
        {
            User usr = new User();
            return usr.Insert(email, firstname, lastname, cardno, cvv, expirydate, password, dateofbirth);
        }

        public User GetUserByEmail(string email)
        {
            User usr = new User();
            return usr.SelectByEmail(email);
        }
    }
}
