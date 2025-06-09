using System;
using System.Collections.Generic;
using System.Text;
using ServerCore;

namespace Server
{
    class GameRoom : IJobQueue
    {
        private List<ClientSession> _session = new List<ClientSession>();

        private JobQueue _jobQueue = new JobQueue();

        private List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

        public void Push(Action job)
        {
            _jobQueue.Push(job);
        }

        public void Flush()
        {
            foreach (ClientSession s in _session)
                s.Send(_pendingList);

            _pendingList.Clear();
        }

        public void Broadcast(ArraySegment<byte> segment)
        {
            _pendingList.Add(segment);
        }

        public void Enter(ClientSession session)
        {
            // 플레이어 할당
            _session.Add(session);
            session.Room = this;

            // 브로드캐스팅
        }

        public void Leave(ClientSession session)
        {
            // 플레이어 제거
            _session.Remove(session);

            // 브로드캐스팅
        }
    }
}
