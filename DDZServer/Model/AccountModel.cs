using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDZServer.Model
{
    public class AccountModel
    {
        public int ID;
        public string Account;
        public string Password;

        public AccountModel() { }
        public AccountModel(int id, string account,string password)
        {
            this.ID = id;
            this.Account = account;
            this.Password = password;
        }
    }
}
