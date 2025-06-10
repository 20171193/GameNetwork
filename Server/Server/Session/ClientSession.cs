using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ServerCore;

namespace Server
{
    class ClientSession : PacketSession
    {
        public int SessionId { get; set; }
        public GameRoom Room { get; set; }

        public override void OnConnected(EndPoint endPoint)
        {
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {

        }

        public override void OnDisconnected(EndPoint endPoint)
        {
        }

        public override void OnSend(int numOfBytes)
        {
        }
    }
}
