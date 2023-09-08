using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.DTO
{
    [Serializable]
    public class PlayerCreateDto
    {
        public string name;
        public PlayerCreateDto() { }
        public PlayerCreateDto(string name)
        {
            this.name = name;
        }
    }

    [Serializable]
    public class PlayerInfoDto
    {
        public int id;
        public int accountID;
        public string name;
        public int beanNum;
        public int winCount;
        public int loseCount;
        public int runCount;

        public int level;
        public int exp;

        public PlayerInfoDto(int id, int accountID, string name, int beanNum, int winCount, int loseCount, int runCount, int level, int exp)
        {
            this.id = id;
            this.accountID = accountID;
            this.name = name;
            this.beanNum = beanNum;
            this.winCount = winCount;
            this.loseCount = loseCount;
            this.runCount = runCount;
            this.level = level;
            this.exp = exp;
        }
    }
}
