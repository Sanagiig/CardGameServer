using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AhpilyServer.Utils.Time
{
    public delegate void TimeDelegate();
    public class TimerModel
    {
        public int ID;
        public long Time;
        public TimeDelegate timeEvent;
        public TimerModel(int id,long time) {
            ID = id;
            Time = time;
        }

        public void Run()
        {

        }
    }
}
