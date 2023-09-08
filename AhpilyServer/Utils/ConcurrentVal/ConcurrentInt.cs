using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AhpilyServer.Utils.ConcurrentVal
{
    public class ConcurrentInt
    {
        private int value;

        public int Value
        {
            get { return value; }
        }
        public ConcurrentInt(int val)
        {
            value = val;
        }

        public ConcurrentInt Add(int v)
        {
            lock (this)
            {
                value += v;
                return this;
            }
        }

        public ConcurrentInt Increase()
        {
            lock (this)
            {
                value++;
                return this;
            }
        }
    }


}
