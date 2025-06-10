using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    // 세션 객체 생성/삭제/탐색 중앙화 (+ID 기반 조회)
    class SessionManager
    {
        static SessionManager _session = new SessionManager();
        public static SessionManager Instance{ get { return _session; } }

        int _sessionId = 0;
        Dictionary<int, ClientSession> _sessions = new Dictionary<int, ClientSession>();

        object _lock = new object();

        public ClientSession Generate()
        {
            lock(_lock)
            {
                int sessionId = ++_sessionId;

                ClientSession session = new ClientSession();
                session.SessionId = sessionId;
                _sessions.Add(sessionId, session);

                Console.WriteLine($"유저 세션 연결됨(ID : {sessionId})");
                return session;
            }
        }

        public ClientSession Find(int id)
        {
            lock(_lock)
            {
                ClientSession session = null;
                _sessions.TryGetValue(id, out session);
                return session;
            }
        }

        public void Remove(ClientSession session)
        {
            lock(_lock)
            {
                _sessions.Remove(session.SessionId);
            }
        }
    }
}
