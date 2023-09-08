using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AhpilyServer.Utils.MonoHelps
{
    public delegate void ExcutionDele();

    public class MonoExcution
    {
        public Mutex mutex;
        private static MonoExcution instance;
        public static MonoExcution Instance
        {
            get
            {
                if (instance == null)
                {
                    return new MonoExcution();
                }
                return instance;
            }
        }
        public MonoExcution()
        {
            mutex = new Mutex();
            instance = this;
        }

        public void Excute(ExcutionDele execute)
        {
            lock (this)
            {
                mutex.WaitOne();
                execute();
                mutex.ReleaseMutex();
            }
        }
    }
}
