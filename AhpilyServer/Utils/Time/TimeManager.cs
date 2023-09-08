using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using AhpilyServer.Utils.ConcurrentVal;
namespace AhpilyServer.Utils.Time
{
    public class TimeManager
    {
        private static TimeManager instance;
        public static TimeManager Instance
        {
            get
            {
                lock (instance)
                {
                    if (instance == null)
                    {
                        instance = new TimeManager();
                    }
                    return instance;
                }
            }

        }

        private ConcurrentDictionary<int,TimerModel> timeDic = new ConcurrentDictionary<int ,TimerModel>();
        private List<int> removeTimerModelIdList = new List<int>();
        private Timer timer;
        private ConcurrentInt id = new ConcurrentInt(0);
        public TimeManager()
        {
            timer = new Timer(1000);
            timer.Elapsed += TimerElapsed;
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            TimerModel tmpTimeModel = null;
            foreach (var id in removeTimerModelIdList)
            {
                timeDic.TryRemove(id, out tmpTimeModel);
            }

            foreach (TimerModel model in timeDic.Values)
            {
                if(model.Time <= DateTime.Now.Ticks)
                {
                    model.Run();
                }
            }
        }

        /// <summary>
        /// 添加事件
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="dele"></param>
        public void AddTimerEvent(DateTime time,TimeDelegate dele)
        {
            long delay = time.Ticks - DateTime.Now.Ticks;
            if(delay >= 0)
            {
                AddTimerEvent(delay, dele);
            }
        }

        public void AddTimerEvent(long delay, TimeDelegate dele)
        {
            TimerModel model = new TimerModel(id.Increase().Value, DateTime.Now.Ticks + delay);
            model.timeEvent = dele;
            timeDic.TryAdd(id.Value, model);
        }
    }
}
