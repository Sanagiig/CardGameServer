using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDZServer.Cache
{
    public class CacheCenter
    {
        private static CacheCenter instance;
        public static CacheCenter Instance
        {
            get
            {
                if (instance == null)
                {
                    return new CacheCenter();
                }
                return instance;
            }
        }

        public AccountCache AccountCache = new AccountCache();
        public PlayerCache PlayerCache = new PlayerCache();
        public RoomCache RoomCache = new RoomCache();
        public FightCache FightCache = new FightCache();
        public CacheCenter()
        {
            instance = this;
        }
    }
}
