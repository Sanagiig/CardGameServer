using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace AhpilyServer
{
    public class PackageTool
    {
        public static byte[] EncodeObj(object obj)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, obj);

            byte[] result = new byte[ms.Length];
            Buffer.BlockCopy(ms.GetBuffer(), 0, result, 0, result.Length);

            ms.Close();
            return result;
        }

        public static object DecodeObj(byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            BinaryFormatter bf = new BinaryFormatter();
            object result = bf.Deserialize(ms);

            ms.Close();
            return result;
        }

        #region 对网络包进行编码 / 解码
        public static byte[] EncodeMessagePacket(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write(buffer.Length);
                    bw.Write(buffer);
                    byte[] data = new byte[ms.Length];
                    Buffer.BlockCopy(ms.GetBuffer(), 0, data, 0, data.Length);
                    return data;
                }
            }
        }

        public static byte[] DecodeMessagePacket(ref List<byte> dataCache)
        {
            if (dataCache.Count < 4)
            {
                // throw new Exception("数据缓存不足4字节,数据包不完整");
                return null;
            }

            using (MemoryStream ms = new MemoryStream(dataCache.ToArray()))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    int len = br.ReadInt32();
                    int remainLen = (int)(ms.Length - ms.Position);
                    if (len > remainLen)
                    {
                        // throw new Exception("数据缺失,数据包长度不构成完整数据");
                        return null;
                    }

                    byte[] data = br.ReadBytes(len);
                    remainLen = (int)(ms.Length - ms.Position);
                    dataCache.Clear();

                    if (remainLen > 0)
                    {
                        dataCache.AddRange(br.ReadBytes(remainLen));
                    }
                    return data;
                }
            }
        }

        public static byte[] EncodeSocketMsg(SocketMsg socketMsg)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(socketMsg.OpCode);
            bw.Write(socketMsg.SubCode);

            if (socketMsg.Value != null)
            {
                bw.Write(EncodeObj(socketMsg.Value));
            }

            byte[] data = new byte[ms.Length];
            Buffer.BlockCopy(ms.GetBuffer(), 0, data, 0, data.Length);

            bw.Close();
            ms.Close();
            return data;
        }

        public static SocketMsg DecodeSocketMsg(byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            BinaryReader br = new BinaryReader(ms);
            SocketMsg msg = new SocketMsg(br.ReadInt32(), br.ReadInt32(), null);
            int remainLen = (int)(ms.Length - ms.Position);

            if (remainLen > 0)
            {
                msg.Value = DecodeObj(br.ReadBytes(remainLen));
            }

            br.Close();
            ms.Close();
            return msg;
        }
        #endregion
    }
}
