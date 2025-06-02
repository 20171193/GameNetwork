using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public abstract class PacketSession : Session
    {
        public const int HeaderSize = 2;

        public sealed override int OnRecv(ArraySegment<byte> buffer)
        {
            int processLen = 0;
            int packetCount = 0;

            while (true)
            {
                if (buffer.Count < HeaderSize)
                    break;

                ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
                if (buffer.Count < dataSize)
                    break;

                // 패킷 조립 후 모아보내기
                OnRecvPacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, dataSize));
                packetCount++;

                processLen += dataSize;
                buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize);
            }
            return processLen;
        }

        public abstract void OnRecvPacket(ArraySegment<byte> buffer);
    }
}
