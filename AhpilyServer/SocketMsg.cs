using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace AhpilyServer
{
    public class SocketMsg
    {
        public int OpCode;
        public int SubCode;
        public object Value;

        public SocketMsg(int opCode, int subCode, object value)
        {
            OpCode = opCode;
            SubCode = subCode;
            Value = value;
        }

        public int GetSize()
        {
            return sizeof(int) + sizeof(int) + (Value as byte[]).Length;
        }
    }
}
