using DDZServer.Model;
using Protocol.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDZServer.Utils
{
    public class DtoTrans
    {
        private static DtoTrans instance;
        public static DtoTrans Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DtoTrans();
                }
                return instance;
            }
        }

        public DtoTrans() { }
        public static PlayerInfoDto GetPlayerInfoDto(PlayerModel m)
        {
            return new PlayerInfoDto(m.Id, m.AccountID, m.Name, m.BeanNum, m.WinCount, m.LoseCount, m.RunCount, m.Level, m.Exp);
        }
    }
}
