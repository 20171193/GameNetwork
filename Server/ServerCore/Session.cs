using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    /// <summary>
    /// Send, Receive 처리
    /// </summary>
    public abstract class Session
    {
        private Socket _socket;
        private int _disconnected = 0;


    }
}
