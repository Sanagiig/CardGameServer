using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.DTO
{
    [Serializable]
    public class AccountDto
    {
        public string account;
        public string psw;
        public AccountDto() { }
        public AccountDto(string account, string psw)
        {
            this.account = account;
            this.psw = psw;
        }
    }
}
