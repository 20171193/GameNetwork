using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ServerCore;

namespace DummyClient
{
    class ServerSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            ;
        }
        public override void OnDisconnected(EndPoint endPoint)
        {
            ;
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            // Todo - PacketManager 연동    
        }

        public override void OnSend(int numOfBytes)
        {
            ;
        }
    }
}
