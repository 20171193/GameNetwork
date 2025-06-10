using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyClient.Session
{
    class SessionManager
    {
        static SessionManager _session = new SessionManager();
        public static SessionManager Instance { get { return _session; } }

        List<ServerSession> _sessions = new List<ServerSession>();

        object _lock = new object();

        public ServerSession Generate()
        {
            lock(_lock)
            {
                ServerSession session = new ServerSession();
                _sessions.Add(session);
                return session;
            }
        }

        public void SendForEach()
        {
            // Todo - 모든 서버 세션에 패킷 전달
        }
    }
}
