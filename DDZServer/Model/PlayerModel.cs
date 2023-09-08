using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDZServer.Model
{
    public class PlayerModel
    {
        public int Id;
        public int AccountID;
        public string Name;
        public int BeanNum;
        public int WinCount;
        public int LoseCount;
        public int RunCount;

        public int Level;
        public int Exp;

        public PlayerModel(int id, string name, int accountID)
        {
            this.Id = id;
            this.Name = name;
            this.AccountID = accountID;
            WinCount = 0;
            LoseCount = 0;
            RunCount = 0;
            Level = 1; 
            Exp = 0;
            BeanNum = 1000;
        }
    }
}
